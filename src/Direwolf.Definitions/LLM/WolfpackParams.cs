using System.Text.Json.Serialization;
using Direwolf.Definitions.Enums;

namespace Direwolf.Definitions.LLM;



public readonly record struct WolfpackParams(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("direwolf")] int Version,
    [property: JsonPropertyName("message_type")] string MessageType,
    [property: JsonPropertyName("result")] string Result,
    [property: JsonPropertyName("uri")] string Uri,
    [property: JsonPropertyName("properties")] IDictionary<string, object>? Properties)
{
    public static WolfpackParams Create(string uri, IDictionary<string, object>? payload)
    {
        return new WolfpackParams(
            "wolfpack",
            "",
            "object",
            1,
            "",
            ResultType.Rejected.ToString(),
            uri,
            payload);
    }
    

    public static IDictionary<string, object> ToDictionary(WolfpackParams p)
    {
        return new Dictionary<string, object>
        {
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