using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.LLM;

namespace Direwolf.Driver.MCP.Tools;

// Hunter exposes APIs that translate between Direwolf and Wolfden RESTful internal APIs
// and the MCP JSON-RCP 2.0 APIs. To do this, Hunter exposes interfaces to operate over
// a DataCache-- Hunter's own Properties Storage. 

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
public static class ResourceHandler
{
    // see McpTool.CreateWolfpack
    // args: [properties] -> data to be stored inside the wolfpack, [name] name of the wolfpack, [description] optional description
    internal static async Task<WolfpackMessage> HandleCreateWolfpack(this Hunter h, object args)
    {
        var json = JsonSerializer.Serialize(args);
        var data = JsonSerializer.Deserialize<JsonElement>(json);
        var name = data.TryGetProperty("properties", out var topValueName) &&
                         topValueName.TryGetProperty("name", out var nameProp)
            ? nameProp.GetString()
            : null;
        var description = data.TryGetProperty("properties", out var topValueDescription) &&
                                 topValueDescription.TryGetProperty("name", out var descriptionProp)
                    ? descriptionProp.GetString()
                    : null;
        
        var wparam = WolfpackMessage.Create(GlobalDictionary.HunterCreate, data) with
        {
            Name = name ?? string.Empty, Description = description ?? string.Empty
        };
        return await h.CreateAsync(in wparam);
    }

    //TODO: Test anonymous types to read wolfpacks.
    // see McpTool.ReadWolfpack
    // args: [keys] -> what to read
    internal static async Task<WolfpackMessage> HandleReadManyWolfpack(this Hunter h, object args)
    {
        var json = JsonSerializer.Serialize(args);
        var data = JsonSerializer.Deserialize<JsonElement>(json);

        var result = data.TryGetProperty("properties", out var props)
                     && props.TryGetProperty("keys", out var keysProp)
        ? keysProp.EnumerateArray()
        : [];
        var wp = WolfpackMessage.Create(GlobalDictionary.HunterReadMany,
            new { keys = result! });
        return await h.GetManyAsync(in wp);
    }

    // see McpTool.ReadWolfpack
    // args: [keys] -> what to read
    // IMPORTANT -> the property is "keys", plural.
    internal static async Task<WolfpackMessage> HandleGetWolfpack(this Hunter h, object args)
    {
        var json = JsonSerializer.Serialize(args);
        var data = JsonSerializer.Deserialize<JsonElement>(json);
        var result = data.TryGetProperty("properties", out var props)
                     && props.TryGetProperty("keys", out var keysProp)
            ? keysProp.GetRawText()
            : null;
        var wp = WolfpackMessage.Create(GlobalDictionary.HunterRead,
            new { key = result });
        return await h.GetAsync(in wp);
    }
    
    // reads everything inside the cache
    // args: [limit] = length of list, [offset] = offset from key, as int
    internal static async Task<WolfpackMessage> HandleListWolfpacks(this Hunter h, object args)
    {
        var json = JsonSerializer.Serialize(args);
        var data = JsonSerializer.Deserialize<JsonElement>(json);
        
        var limit = data.TryGetProperty("properties", out var limitPropTop)
            && limitPropTop.TryGetProperty("limit", out var limitPropLimit)
            ? limitPropTop.GetInt32()
            : 0;
        var offset = data.TryGetProperty("properties", out var offsetPropTop)
                    && limitPropTop.TryGetProperty("offset", out var offsetPropLimit)
                    ? limitPropTop.GetInt32()
                    : 0;
        
        var wp = WolfpackMessage.Create(GlobalDictionary.HunterTools,
            new { limit, offset });
        return await h.GetManyAsync(in wp); // change to ListMany => list the cache.
    }

    // args: [key] -> which one, [value] -> to what
    // to update, pass directly a dictionary.
    internal static async Task<WolfpackMessage> HandleUpdateWolfpack(this Hunter h, object args)
    {
        var json = JsonSerializer.Serialize(args);
        var data = JsonSerializer.Deserialize<JsonElement>(json);
        
        var keys = data.TryGetProperty("properties", out var updatePropTop)
                    && updatePropTop.TryGetProperty("keys", out var updateProp)
            ? JsonSerializer.Deserialize<Dictionary<string, object>>(updateProp.GetRawText())
            : null;
        
        var wp = WolfpackMessage.Create(GlobalDictionary.HunterCreate, keys);
        return await h.UpdateAsync(in wp);
    }
    
    
    internal static async Task<WolfpackMessage> HandleDeleteWolfpack(this Hunter h, object args)
    {
    
        var json = JsonSerializer.Serialize(args);
        var data = JsonSerializer.Deserialize<JsonElement>(json);
            var keys = data.TryGetProperty("properties", out var deletePropTop)
                           && deletePropTop.TryGetProperty("keys", out var deleteProp)
                    ? JsonSerializer.Deserialize<IEnumerable<string>>(deleteProp.GetRawText())
                    : null; 
        var wp = WolfpackMessage.Create(GlobalDictionary.HunterDelete,
            new { keys });
        return await h.UpdateAsync(in wp);
    }

    // args: none
    

    // // this will use the uri to get data:
    // //      inside parameters, will look for args each one needs. 
    // // args: [uri] which one you want
    // internal static async Task<McpResponse> HandleGetTools(this McpDriver drv, McpRequest request)
    // {
    //     var parameters = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(request.Params));
    //     var uri = parameters.TryGetProperty("params", out var urisPropTop)
    //                && urisPropTop.TryGetProperty("uri", out var urisProp)
    //         ? urisProp.GetString()
    //         : null; 
    //     var wpUri = new WolfpackUri(uri!);
    //     var wpack = WolfpackMessage.Create(uri!, null);
    //     try
    //     {
    //         // get tools
    //         McpDriver._toolHandlers.TryGetValue()
    //         var listTools = await 
    //         var availableTools = (IDictionary<string, List<McpTool>>?)listTools.Result;
    //         // if (availableTools is not null)
    //         //     return wpUri.GetPath() switch
    //         //     {
    //         //         GlobalDictionary.HunterCreate => await Task.FromResult(McpResponse.Create(ResourceDefinitions.CreateWolfpack, null)),
    //         //         GlobalDictionary.HunterRead => await Task.FromResult(McpResponse.Create(ResourceDefinitions.ReadWolfpack, null)),
    //         //         GlobalDictionary.HunterReadMany => await Task.FromResult(
    //         //             McpResponse.Create(ResourceDefinitions.GetWolfpackMany, null)),
    //         //         GlobalDictionary.HunterUpdate => await Task.FromResult(McpResponse.Create(ResourceDefinitions.UpdateWolfpack, null)),
    //         //         GlobalDictionary.HunterDelete => await Task.FromResult(McpResponse.Create(ResourceDefinitions.DeleteWolfpack, null)),
    //         //         GlobalDictionary.HunterLlmAnalyze=> await Task.FromResult(McpResponse.Create(ResourceDefinitions.AiAnalyzeWolfpack,
    //         //             null)),
    //         //         GlobalDictionary.HunterLlmGenerate => await Task.FromResult(McpResponse.Create(ResourceDefinitions.AiGenerateWolfpack,
    //         //             null)),
    //         //         GlobalDictionary.HunterResources => await Task.FromResult(McpResponse.Create(HandleGetResources(), null)),
    //         //         _ => await Task.FromResult(new McpResponse(request.Id, "Could not read resource.", new {code = -32063, message = "Returned default value at GetUri()."}))
    //         //     };
    //     }
    //     catch (Exception e)
    //     {
    //         return new McpResponse(request.Id, "Could not read resource.", new {code = -32063, message = "An exception has occured.", data = e.Message});
    //     }
    //
    //     return new McpResponse(request.Id, "Could not read resource.", new {code = -32063, message = "Could not read resource."});
    // }

    public  static WolfpackMessage CreateErrorResponse(int code, string message, object? data = null)
    {
        return WolfpackMessage.Create(GlobalDictionary.HunterErrors, new { code, message, data = data! });
    }

    // internal static Task<WolfpackMessage> HandleGetResources(in WolfpackMessage request)
    // {
    //     var propertyNames = new List<McpTool>
    //     {
    //         ResourceDefinitions.CreateWolfpack,
    //         ResourceDefinitions.ReadWolfpack,
    //         ResourceDefinitions.UpdateWolfpack,
    //         ResourceDefinitions.DeleteWolfpack,
    //         ResourceDefinitions.AiAnalyzeWolfpack,
    //         ResourceDefinitions.AiGenerateWolfpack
    //     };
    //     return Task.FromResult(request with
    //     {
    //         MessageType = MessageResponse.Result.ToString(),
    //         Result = new { tools = propertyNames }
    //     });
    // }
}