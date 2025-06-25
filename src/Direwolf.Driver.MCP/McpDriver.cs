using System.Diagnostics;
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
public sealed partial class McpDriver
{
    /// <summary>
    /// Reference to a Direwolf instance,
    /// </summary>
    private static Hunter? _hunter;

    private static AnthropicClient? _anthropicClient;
    private static McpDriver? _instance;
    private static StreamingConsole? _console;
    private static readonly object Lock = new();
    internal static Dictionary<string, Func<object, Task<WolfpackMessage>>> _toolHandlers = new();
    internal static Dictionary<string, Func<object, Task<WolfpackMessage>>> _resourceHandlers = new();
    
    private McpDriver(Hunter hunter)
    {
        _hunter = hunter;
    }

    public static McpDriver GetInstance(Hunter hunter, string anthropicApiKey)
    {
        if (_instance is not null) return _instance;
        lock (Lock)
        {
            if (_instance is not null) return _instance;
            _instance = new McpDriver(hunter);
            _console = new StreamingConsole();
            _toolHandlers = HandleInitTools();
            _resourceHandlers = HandleInitResources();
            _anthropicClient = new AnthropicClient(anthropicApiKey);

            return _instance;
        }
    }

    public static void ToConsole(string text) => _console?.StreamMessage(text);
    
    // public static async Task<WolfpackMessage> HandleRequest(WolfpackMessage wp)
    // {
    //     try
    //     {
    //         var props = (IDictionary<string, object>)wp.Properties!;
    //         return props.Keys.FirstOrDefault() switch
    //         {
    //             "ai_analyze" => await ResourceAiAnalyzer(wp),
    //             "ai_generate" => await ResourceAiGenerator(wp),
    //             _ => ResourceHandler.CreateErrorResponse(-32603, $"Resource not found: {wp.Properties}", wp)
    //         };
    //     }
    //     catch (Exception ex)
    //     {
    //         return ResourceHandler.CreateErrorResponse(-32603, "Internal error", ex.Message);
    //     }
    // }
    
}

/// <summary>
/// Handlers
/// </summary>
public sealed partial class McpDriver
{
    private static Dictionary<string, Func<object, Task<WolfpackMessage>>> HandleInitTools()
    {
        return new Dictionary<string, Func<object, Task<WolfpackMessage>>>
        {
            ["create"] = _hunter!.HandleCreateWolfpack,
            ["get"] = _hunter.HandleGetWolfpack,
            ["get-many"] = _hunter.HandleReadManyWolfpack,
            ["update"] = _hunter.HandleUpdateWolfpack,
            ["delete"] = _hunter.HandleDeleteWolfpack
        };
    }
    
    private static Dictionary<string, Func<object, Task<WolfpackMessage>>> HandleInitResources()
    {
        return new Dictionary<string, Func<object, Task<WolfpackMessage>>>
        {
            ["ai-analyze"] = ResourceAiAnalyzer,
            ["ai-generate"] = ResourceAiGenerator
        };
    }
    
    public static async Task<WolfpackMessage> HandleGetResources()
        {
            return await Task.FromResult(WolfpackMessage.Create(GlobalDictionary.HunterResources, new{name = "list_tools", description = "List of all tools.", mimeType = "application/json"})) with
            {
                Result = _resourceHandlers
            };
        }
    
    public static async Task<WolfpackMessage> HandleGetTools()
        {
            return await Task.FromResult(WolfpackMessage.Create(GlobalDictionary.HunterResources, new{name = "list_resources", description = "List of all resources.", mimeType = "application/json"})) with
            {
                Result = _toolHandlers
            };
        }
}


/// <summary>
/// Resources
/// </summary>
public sealed partial class McpDriver
{
    private const string TestJson = """
                                        "Id": {
                                            "Length": 16,
                                            "Value": "cmbyiyd1700c62cchc2jkillwhpkplblvazjl"
                                        },
                                        "Method": 0,
                                        "Name": "Model Health Stats",
                                        "Parameters": null,
                                        "Title": [
                                            {
                                                "Id": {
                                                    "Length": 1,
                                                    "Value": "cmbyiyd1700000000hqxv0z18ge4i9n9f083s"
                                                },
                                                "MessageType": 1,
                                                "Result": 1,
                                                "DataType": 9,
                                                "RequestType": 0,
                                                "Payload": {
                                                    "viewsInsideDocument": 302,
                                                    "notInSheets": 0,
                                                    "annotativeElements": 1967,
                                                    "externalRefs": 233120,
                                                    "modelGroups": 49,
                                                    "detailGroups": 0,
                                                    "designOptions": 6,
                                                    "levels": 18,
                                                    "grids": 28,
                                                    "warns": 49,
                                                    "unenclosedRoom": 0,
                                                    "viewports": 0,
                                                    "unconnectedDucts": 0,
                                                    "unconnectedPipes": 0,
                                                    "unconnectedElectrical": 0,
                                                    "nonNativeStyles": 2,
                                                    "isFlipped": 1820,
                                                    "worksetElementCount": 1
                                                },
                                                "Description": "ModelHealthIndicators"
                                            }
                                        ]
                                    }
                                    """;
    public static async Task<WolfpackMessage> AiAnalyze(string keys) => await ResourceAiAnalyzer(keys);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="h"></param>
    /// <param name="args">options (any additional context), key (element to analyze)</param>
    /// <returns></returns>
    private static async Task<WolfpackMessage> ResourceAiAnalyzer(object args)
    {
        var wp = WolfpackMessage.Create("wolfpack://direwolf/hunter/tools/llm/analyze", new Dictionary<string, object>
        {
            ["keys"] = args
        });

        // var response = await _hunter!.GetAsync(in wp);
        // var jsonResponse = JsonSerializer.Serialize(wp.Result);
        var jsonResponse = TestJson;
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
    private static async Task<WolfpackMessage> ResourceAiGenerator(object args)
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