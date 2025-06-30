using System.Text.Json.Serialization;
using Direwolf.Definitions.Enums;

namespace Direwolf.Definitions.LLM;

public readonly record struct WolfpackMessage(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("direwolf")] int Version,
    [property: JsonPropertyName("message_type")] string MessageType,
    [property: JsonPropertyName("result")] object? Result,
    [property: JsonPropertyName("uri")] string Uri,
    [property: JsonPropertyName("parameters")] object? Properties = null)
{
    [JsonPropertyOrder(0), JsonPropertyName("jsonrpc")] public const string JsonRpc = "2.0";
    [JsonPropertyName("createdAt")] public readonly DateTime CreatedAt = DateTime.UtcNow;
    public static WolfpackMessage Create(string uri, object? properties = null)
    {
        return new WolfpackMessage(
            Cuid.Create().Value!,
            "wolfpack",
            "",
            "object",
            1,
            "",
            ResultType.Rejected.ToString(),
            uri,
            properties);
    }
    
    public static IDictionary<string, object> ToDictionary(WolfpackMessage p)
    {
        return new Dictionary<string, object>
        {
            ["id"] = p.Id,
            ["name"] = p.Name,
            ["description"] = p.Description,
            ["type"] = p.Type,
            ["version"] = p.Version,
            ["message_type"] = p.MessageType,
            ["uri"] = p.Uri,
            ["properties"] = p.Properties!
        };
    }
}

// already implemented