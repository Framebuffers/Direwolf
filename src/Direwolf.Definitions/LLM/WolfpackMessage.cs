using System.Text.Json.Serialization;
using Direwolf.Definitions.Enums;

namespace Direwolf.Definitions.LLM;



public readonly record struct WolfpackMessage(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("direwolf")] int Version,
    [property: JsonPropertyName("message_type")] string MessageType,
    [property: JsonPropertyName("result")] object? Result,
    [property: JsonPropertyName("uri")] string Uri,
    [property: JsonPropertyName("parameters")] IDictionary<string, object>? Parameters = null)
{
    [JsonPropertyOrder(0), JsonPropertyName("jsonrpc")] public const string JsonRpc = "2.0";
    public static WolfpackMessage Create(string uri, IDictionary<string, object>? payload)
    {
        return new WolfpackMessage(
            "wolfpack",
            "",
            "object",
            1,
            "",
            ResultType.Rejected.ToString(),
            uri,
            payload);
    }
    

    public static IDictionary<string, object> ToDictionary(WolfpackMessage p)
    {
        return new Dictionary<string, object>
        {
            ["name"] = p.Name,
            ["description"] = p.Description,
            ["type"] = p.Type,
            ["version"] = p.Version,
            ["message_type"] = p.MessageType,
            ["uri"] = p.Uri,
            ["properties"] = p.Parameters!
        };
    }
}

// already implemented