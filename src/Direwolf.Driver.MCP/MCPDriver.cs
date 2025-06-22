using System.Text.Json;
using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using Direwolf.Definitions;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.LLM;
using Direwolf.Driver.MCP.Tools;
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
    private readonly Dictionary<string, Func<object, Task<WolfpackMessage>>> _handlers = new();
    
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

    public async Task<WolfpackMessage> HandleRequest(WolfpackMessage request)
    {
        try
        {
            if (request.Parameters is null)
                return OperationHandler.CreateErrorResponse(-32603, "Request Parameters are null", request);
            var method = (IDictionary<string, object>)request.Parameters;
            return method["method"] switch
            {
                "initialize" => await OperationHandler.HandleInitialize(request),
                "tools" => await OperationHandler.HandleListTools(request),
                "tools/get" => await _hunter!.HandleGetWolfpack(request),
                "tools/get-many" => await _hunter!.HandleGetManyWolfpack(request),
                "tools/list" => await OperationHandler.HandleListTools(request),
                "tools/create" => await _hunter!.HandleCreateWolfpack(request),
                "tools/update" => await _hunter!.HandleUpdateWolfpack(request),
                "tools/delete" => await _hunter!.HandleDeleteWolfpack(request),
                "tools/llm/analyze" => await HandleAiAnalizeWolfpack(request),
                "tools/llm/generate" => await HandleAiGenerateWolfpack(request),
                _ => OperationHandler.CreateErrorResponse(-32601, "Method not found")
            };
        }
        catch(Exception ex)
        {
            return OperationHandler.CreateErrorResponse(-32603, "Internal error", ex.Message);
        }
    }
}

//
public sealed partial class MCPDriver
{
    private Dictionary<string, Func<object, Task<WolfpackMessage>>> InitToolHandlers()
    {
        return new()
        {
            ["create"] = _hunter!.HandleCreateWolfpack,
            ["get"] = _hunter.HandleGetWolfpack,
            ["get-many"] = _hunter.HandleGetManyWolfpack,
            ["update"] = _hunter.HandleUpdateWolfpack,
            ["delete"] = _hunter.HandleDeleteWolfpack,
            ["ai-analyze"] = HandleAiAnalizeWolfpack,
            ["ai-generate"] = HandleAiGenerateWolfpack
        };
    }
}

public sealed partial class MCPDriver
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="h"></param>
    /// <param name="args">options (any additional context), key (element to analyze)</param>
    /// <returns></returns>
    private async Task<WolfpackMessage> HandleAiAnalizeWolfpack(object args)
    {
        var json = JsonSerializer.Serialize(args);
        var data = JsonSerializer.Deserialize<JsonElement>(json);
        var elementToAnalyze = data.GetProperty("key");
        var options = data.TryGetProperty("options", out var props)
            ? JsonSerializer.Deserialize<Dictionary<string, object>>(props.GetRawText())
            : null;
        var wp = WolfpackMessage.Create("wolfpack://direwolf/hunter/tools/llm/analyze", new Dictionary<string, object>
        {
            ["key"] = elementToAnalyze, ["options"] = options! // token limit, model, etc.
        });
        var response = await _hunter!.GetAsync(in wp);
        var jsonResponse = JsonSerializer.Serialize(response.Result);
        var prompt =
            $"Analyze this entity and provide insights about its structure, potential use cases, and suggestions for improvement:\n\n{jsonResponse}";
        var messages = new List<Message> { new(RoleType.User, prompt) };
        var parameters = new MessageParameters
        {
            Messages = messages,
            MaxTokens = 1024,
            Model = AnthropicModels.Claude4Sonnet,
            Stream = false,
            Temperature = 1.0m
        };
        var claudeResponse = await _anthropicClient?.Messages.GetClaudeMessageAsync(parameters)!;
        wp =  wp with { Result = claudeResponse };
      
        return await Task.FromResult(wp);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="h"></param>
    /// <param name="args">prompt: (text prompt), options: (any additional context). Set to 512 tokens max.</param>
    /// <returns></returns>
    private static async Task<WolfpackMessage> HandleAiGenerateWolfpack(object args)
    {
        var json = JsonSerializer.Serialize(args);
        var data = JsonSerializer.Deserialize<JsonElement>(json);
        var k = data.GetProperty("prompt");
        var options = data.TryGetProperty("options", out var props)
            ? JsonSerializer.Deserialize<Dictionary<string, object>>(props.GetRawText())
            : null;
        var wp = WolfpackMessage.Create("wolfpack://direwolf/hunter/tools/llm/generate", new Dictionary<string, object>
        {
            ["prompt"] = k, ["options"] = options! // token limit, model, etc.
        });
        var response = await _hunter!.GetAsync(in wp);
        var jsonResponse = JsonSerializer.Serialize(response.Result);
        var prompt = $"{k}:\n\n{jsonResponse}";
        var messages = new List<Message> { new(RoleType.User, prompt) };
        var parameters = new MessageParameters
        {
            Messages = messages,
            MaxTokens = 512,
            Model = AnthropicModels.Claude4Sonnet,
            Stream = false,
            Temperature = 1.0m
        };
        var claudeResponse = await _anthropicClient?.Messages.GetClaudeMessageAsync(parameters)!;
        wp = wp with { Result = claudeResponse };
        return await Task.FromResult(wp);
    }
}