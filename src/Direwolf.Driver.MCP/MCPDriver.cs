using System.Text.Json;
using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using Direwolf.Definitions;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.LLM;
using MessageResponse = Direwolf.Definitions.Enums.MessageResponse;

namespace Direwolf.Driver.MCP;

//TODO: connect it to Hunter on the Direwolf side.
/// <summary>
/// Acts as the MCP driver for Direwolf. MCP drivers provide context, tools, prompts and utilities to communicate
/// with other LLMs that implement this protocol. <see cref="Direwolf"/> drivers are assemblies that use the Wolfpack format
/// to communicate and transform data. This driver provides access to the Direwolf context, templates to communicate with
/// BIM applications and methods to pipe information back to the client through the Host.
/// </summary>
public sealed partial class MCPDriver
{
    /// <summary>
    /// Reference to a Direwolf instance,
    /// </summary>
    private static Hunter? _hunter;
    private static AnthropicClient? _anthropicClient;
    private static MCPDriver? _instance;
    private static readonly object Lock = new();
    private readonly Dictionary<string, Func<object, Task<object>>> _handlers = new();  
    
    private MCPDriver(Hunter hunter)
    {
        _hunter = hunter;
    }

    public static MCPDriver GetInstance(Hunter hunter, string anthropicApiKey)
    {
        if (_instance is not null) return _instance;
        lock (Lock)
        {
            if (_instance is not null) return _instance;
            _instance = new MCPDriver(hunter);
            _anthropicClient = new AnthropicClient(anthropicApiKey);
            return _instance;
        }
    }
    
    
}
//
public sealed partial class MCPDriver
{
    internal static async Task<Wolfpack> HandleAiAnalizeWolfpack(Hunter h, object args)
    {
        var json = JsonSerializer.Serialize(args);
        var data = JsonSerializer.Deserialize<JsonElement>(json);
        var elementToAnalyze = data.GetProperty("key");
        var options = data.TryGetProperty("options", out var props)
            ? JsonSerializer.Deserialize<Dictionary<string, object>>(props.GetRawText())
            : null;
        var wp = WolfpackMessage.Create("wolfpack://direwolf/hunter/llm/analyze", new Dictionary<string, object>
        {
            ["key"] = elementToAnalyze, ["options"] = options! // token limit, model, etc.
        });
        
        var response = await h.GetAsync(in wp);
        var jsonResponse = JsonSerializer.Serialize(response.McpResponseResult);
        var prompt =
            $"Analyze this entity and provide insights about its structure, potential use cases, and suggestions for improvement:\n\n{jsonResponse}";
        var messages = new List<Message>
        {
            new(RoleType.User, prompt)
        };

        var parameters = new MessageParameters
        {
            Messages = messages,
            MaxTokens = 1024,
            Model = AnthropicModels.Claude4Sonnet,
            Stream = false,
            Temperature = 1.0m
        };

        var claudeResponse = await _anthropicClient?.Messages.GetClaudeMessageAsync(parameters)!;

        wp = wp with
        {
            Result = claudeResponse
        };

        response = response with
        {
            Name = wp.Name,
            McpResponseResult = wp.Result,
            Description = wp.Description,
            MessageResponse = MessageResponse.Result,
            Parameters = wp.Parameters,
            RequestType = RequestType.Get
        };

        return await Task.FromResult(response);
    }

    internal static async Task<object> HandleAiGenertateWolfpack(Hunter h, object args)
    {
        var json = JsonSerializer.Serialize(args);
        var data = JsonSerializer.Deserialize<JsonElement>(json);
        var k = data.GetProperty("prompt");
        var options = data.TryGetProperty("options", out var props)
            ? JsonSerializer.Deserialize<Dictionary<string, object>>(props.GetRawText())
            : null;
        var wp = WolfpackMessage.Create("wolfpack://direwolf/hunter/llm/generate", new Dictionary<string, object>
        {
            ["prompt"] = k, ["options"] = options! // token limit, model, etc.
        });
        var response = await h.GetAsync(in wp);
        var jsonResponse = JsonSerializer.Serialize(response.McpResponseResult);
        var prompt =
            $"{k}:\n\n{jsonResponse}";
        var messages = new List<Message>
        {
            new(RoleType.User, prompt)
        };

        var parameters = new MessageParameters
        {
            Messages = messages,
            MaxTokens = 512,
            Model = AnthropicModels.Claude4Sonnet,
            Stream = false,
            Temperature = 1.0m
        };

        var claudeResponse = await _anthropicClient?.Messages.GetClaudeMessageAsync(parameters)!;

        wp = wp with
        {
            Result = claudeResponse
        };

        return await Task.FromResult(wp);
    }
}