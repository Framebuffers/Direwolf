using System.Diagnostics;
using System.Text.Json;
using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.LLM;

namespace Direwolf.Driver.MCP.Tools;

// Hunter exposes APIs that translate between Direwolf and Wolfden RESTful internal APIs
// and the MCP JSON-RCP 2.0 APIs. To do this, Hunter exposes interfaces to operate over
// a DataCache-- Hunter's own Parameters Storage. 

// All raw request should have their data as a value of key ["payload"]

// These interfaces are called Tools. Hunter will expose six (6) of them: the basic operators for the cache.
// Inside each JSON-RPC message, Hunter will look for different keys depending on the kind of operation:
// For built-in tools:
//      Create              -> ["name", "description", "payload"]
//      Get                 -> ["key"]
//      Update              -> ["key", "payload"]
//      Delete              -> ["key"]
//      AiAnalyze           -> ["key"]
//      AiGenerate          -> ["prompt"]

// These tools will create a WolfpackMessage record, wrapping the MCP message with all necessary context.
// To perform any operation, all required parameters are held inside a field of type Dictionary<string, object>?
// Its contents define the result itself of the operation. For example, these are examples of operation results 
// and their value inside this Dictionary:
//      No results found    -> null
//      Error found         -> TaskAsException

// When performing an operation, it will convert the result to JSON and look for a [payload] property.
// Depending on the operation, it will perform a Task defined inside Hunter. each task will return a
// different result specified by the state of this dictionary. For each operation, expected results are:
//      Create              -> ID's of the new elements.
//      Get                 -> Elements and their payload
//      Update              -> Elements *that could not* be updated. Null if successful.
//      Delete              -> Elements that could not be deleted. Null if successful.
//      AiAnalyze           -> Payload with results. Null if *unsuccessful*
//      AiGenerate          -> Payload with results. Null if *unsuccessful*

// Internally, operations are all processed using one big private method, that takes the data in, and the operation
// as an Enum entry. Therefore, all operations are contained on a single place. Tasks are just wrappers of this method.
// Following the same schema, Decompress will take the args of any Operation and unpack it to JSON, and return a 
// Wolfpack.

// This is because most of the logic is shared between all Operations, because all follow the same format.
// Decompress() doesn't care about the kind of operation, it will just unpack the args and return a Wolfpack of whatever
// is inside, in hopes that not much has to be altered if the format changes.
public static class OperationHandler
{
    // see McpTool.CreateWolfpack
    internal static async Task<WolfpackMessage> HandleCreateWolfpack(this Hunter h, object args)
    {
        var json = JsonSerializer.Serialize(args);
        var data = JsonSerializer.Deserialize<JsonElement>(json);
        var properties = data.TryGetProperty("payload", out var props)
            ? JsonSerializer.Deserialize<Dictionary<string, object>>(props.GetRawText())
            : null;
        var wparam = WolfpackMessage.Create("wolfpack://direwolf/hunter/tools/create", properties) with
        {
            Name = data.GetProperty("name").GetString()!, Description = data.GetProperty("description").GetString()!
        };
        return await h.CreateAsync(in wparam);
    }

    // see McpTool.GetWolfpack
    internal static async Task<WolfpackMessage> HandleGetManyWolfpack(this Hunter h, object args)
    {
        var json = JsonSerializer.Serialize(args);
        var data = JsonSerializer.Deserialize<JsonElement>(json);

        // var limit = data.TryGetProperty("limit", out var limitProp) ? limitProp.GetInt32() : 100;
        // var offset = data.TryGetProperty("offset", out var offsetProp) ? offsetProp.GetInt32() : 0;
        var result = data.TryGetProperty("keys", out var props)
            ? JsonSerializer.Deserialize<Dictionary<string, object>>(props.GetRawText())
            : null;
        var wp = WolfpackMessage.Create("wolfpack://direwolf/hunter/tools/get-many",
            new Dictionary<string, object> { ["keys"] = result!.Values.ToArray() });
        return await h.GetManyAsync(in wp);
    }

    // see McpTool.GetWolfpack
    internal static async Task<WolfpackMessage> HandleGetWolfpack(this Hunter h, object args)
    {
        var json = JsonSerializer.Serialize(args);
        var data = JsonSerializer.Deserialize<JsonElement>(json);
        var result = data.TryGetProperty("key", out var props)
            ? JsonSerializer.Deserialize<Dictionary<string, object>>(props.GetRawText())
            : null;
        var wp = WolfpackMessage.Create("wolfpack://direwolf/hunter/tools/get",
            new Dictionary<string, object> { ["key"] = result!.Values.ToArray() });
        return await h.GetAsync(in wp);
    }

    internal static async Task<WolfpackMessage> HandleListWolfpacks(this Hunter h, object args)
    {
        var json = JsonSerializer.Serialize(args);
        var data = JsonSerializer.Deserialize<JsonElement>(json);
        var limit = data.TryGetProperty("limit", out var limitFound) ? limitFound.GetInt32() : 0;
        var offset = data.TryGetProperty("offset", out var offsetFound) ? offsetFound.GetInt32() : 0;
        var wp = WolfpackMessage.Create("wolfpack://direwolf/hunter/tools/list",
            new Dictionary<string, object> { ["limit"] = limit, ["offset"] = offset });
        return await h.GetManyAsync(in wp); // change to ListMany => list the cache.
    }

    internal static async Task<WolfpackMessage> HandleUpdateWolfpack(this Hunter h, object args)
    {
        var json = JsonSerializer.Serialize(args);
        var data = JsonSerializer.Deserialize<JsonElement>(json);
        var k = data.GetProperty("key");
        var v = data.TryGetProperty("value", out var value) ? value.GetRawText() : null;
        var wp = WolfpackMessage.Create("wolfpack://direwolf/hunter/tools/update",
            new Dictionary<string, object> { ["key"] = k, ["value"] = v });
        return await h.UpdateAsync(in wp);
    }

    internal static async Task<WolfpackMessage> HandleDeleteWolfpack(this Hunter h, object args)
    {
        var json = JsonSerializer.Serialize(args);
        var data = JsonSerializer.Deserialize<JsonElement>(json);
        var k = data.GetProperty("key");
        var wp = WolfpackMessage.Create("wolfpack://direwolf/hunter/tools/delete",
            new Dictionary<string, object> { ["key"] = k });
        return await h.UpdateAsync(in wp);
    }

    internal static async Task<WolfpackMessage> HandleListResources()
    {
        return await Task.FromResult(WolfpackMessage.Create("wolfpack://direwolf/hunter/resources/all", new{name = "All entities", description = "List of all tools.", mimeType = "application/json"}));
    }

    // this will use the uri to get data:
    //      it will use the URI detailed inside the args:
    //          available = wolfpack://direwolf/hunter/[create, getMany, get, list, update, delete, llm/analyze, llm/generate
    //      inside parameters, it will look for args each one needs. 
    internal static async Task<McpResponse> HandleReadResource(McpRequest request)
    {
        var parameters = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(request.Params));
        var uri = parameters.GetProperty("uri").GetString() ?? "wolfpack://direwolf/hunter";
        var wpUri = new WolfpackUri(uri);
        var wpack = WolfpackMessage.Create(uri, null);
        try
        {
            // get tools
            var listTools = await HandleListTools(wpack);
            var availableTools = (IDictionary<string, List<McpTool>>?)listTools.Result;
            if (availableTools is not null)
                return wpUri.GetPath() switch
                {
                    "tools/create" => await Task.FromResult(McpResponse.Create(ToolDefinitions.CreateWolfpack, null)),
                    "tools/get" => await Task.FromResult(McpResponse.Create(ToolDefinitions.GetWolfpack, null)),
                    "tools/get-many" => await Task.FromResult(
                        McpResponse.Create(ToolDefinitions.GetWolfpackMany, null)),
                    "tools/update" => await Task.FromResult(McpResponse.Create(ToolDefinitions.UpdateWolfpack, null)),
                    "tools/delete" => await Task.FromResult(McpResponse.Create(ToolDefinitions.DeleteWolfpack, null)),
                    "tools/llm/ai-analyze" => await Task.FromResult(McpResponse.Create(ToolDefinitions.AiAnalyzeWolfpack,
                        null)),
                    "tools/llm/ai-generate" => await Task.FromResult(McpResponse.Create(ToolDefinitions.AiGenerateWolfpack,
                        null)),
                    _ => new McpResponse(request.Id, "Could not read resource.",
                        new { code = -32063, message = "URI not found.", data = wpUri.GetPath() })
                };
        }
        catch (Exception e)
        {
            return new McpResponse(request.Id, "Could not read resource.", new {code = -32063, message = "An exception has occured.", data = e.Message});
        }

        return new McpResponse(request.Id, "Could not read resource.", new {code = -32063, message = "Could not read resource."});
    }

    public  static WolfpackMessage CreateErrorResponse(int code, string message, object? data = null)
    {
        return WolfpackMessage.Create("wolfpack://direwolf/hunter", new { code, message, data = data! });
    }

    internal static Task<WolfpackMessage> HandleListTools(in WolfpackMessage request)
    {
        var propertyNames = new List<McpTool>
        {
            ToolDefinitions.CreateWolfpack,
            ToolDefinitions.GetWolfpack,
            ToolDefinitions.UpdateWolfpack,
            ToolDefinitions.DeleteWolfpack,
            ToolDefinitions.AiAnalyzeWolfpack,
            ToolDefinitions.AiGenerateWolfpack
        };
        return Task.FromResult(request with
        {
            MessageType = MessageResponse.Result.ToString(),
            Result = new { tools = propertyNames }
        });
    }

    internal static Task<WolfpackMessage> HandleInitialize(in WolfpackMessage request)
    {
        Debug.Write($"Inside {nameof(HandleInitialize)}:{JsonSerializer.Serialize(request)}");
        return Task.FromResult(request with
        {
            MessageType = MessageResponse.Result.ToString(),
            Result = new
            {
                protocolVersion = "2025-06-18",
                capabilities = new { tools = new { }, resources = new { } },
                serverinfo = new { name = "direwolf-hunter", version = "v0.2-alpha" }
            }
        });
    }
}