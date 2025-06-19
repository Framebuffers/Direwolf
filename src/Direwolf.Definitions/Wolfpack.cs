using System.Reflection;
using System.Runtime.Caching;
using System.Text.Json;
using System.Text.Json.Serialization;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.LLM;
using Direwolf.Definitions.Serialization;

namespace Direwolf.Definitions;

//TODO: move payload to params and everything pointing to it. it is now inside the params.
public readonly record struct Wolfpack(
    [property: JsonPropertyName("id")] Cuid Id, // mcp 
    [property: JsonIgnore] RequestType? Method, // direwolf
    [property: JsonPropertyName("name")] string? Name, // mcp
    [property: JsonPropertyName("@params")]
    WolfpackParams Params, // mcp
    [property: JsonPropertyName("description")] // mcp
    string? Description)
{
    [JsonPropertyName("jsonrpc"), JsonPropertyOrder(0)] public const string JsonRpc = "2.0";
    [JsonPropertyName("wolfpack")] public const string WolfpackVersion = "1.0";
    [JsonPropertyName("createdAt")] public DateTimeOffset? CreatedAt => Id.GetDateTimeCreation();
    [JsonPropertyName("updatedAt")] public DateTime UpdatedAt => DateTime.UtcNow;
    public static Wolfpack Create(Cuid? id, 
        RequestType requestType, 
        string? name,
        WolfpackParams @params, 
        string? description = null)
    {
        return new Wolfpack(id ?? Cuid.Create(), 
            requestType, 
            name, 
            @params, 
            description);
    }

    public static Wolfpack Create(RequestType requestType, 
        string? name, 
        WolfpackParams @params,
        string? description = null)
    {
        return new Wolfpack(Cuid.Create(), 
            requestType, 
            name, 
            @params, 
            description);
    }

    public static Wolfpack Get(string? name, 
        WolfpackParams @params, 
        out McpResourceContainer[]? data,
        string? description = null)
    {
        data = null;
        return new Wolfpack(Cuid.Create(), 
            RequestType.Get, 
            name, 
            @params, 
            description);
    }

    // public static Wolfpack AsPrompt(Wolfpack w, 
    //     string promptName, 
    //     string promptDescription, 
    //     DataType payloadType,
    //     Howl[]? data,
    //     string uri)
    // {
    //     var args = new WolfpackParams(promptName, 
    //         promptDescription, 
    //         payloadType.ToString(), 
    //         1, 
    //         MessageType.Request.ToString(),
    //         ResultType.Rejected.ToString(), 
    //         uri);
    //     
    //     var wp = new Wolfpack(Cuid.Create(), 
    //         RequestType.Get, 
    //         promptName, 
    //         args, 
    //         data?.Select(McpResourceContainer.Create)
    //             .ToArray(),
    //         promptDescription);
    //     return wp;
    // }

    public static CacheItem AsCacheItem(Wolfpack w)
    {
        return new CacheItem(w.Id.Value, w);
    }


}
