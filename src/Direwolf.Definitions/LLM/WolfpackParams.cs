using System.Text.Json.Serialization;

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
    public static WolfpackParams Create(Howl h, string uri, IDictionary<string, object>? payload)
    {
        return new WolfpackParams(
            h.Name ?? null!, 
            h.Description ?? null!, 
            Howl.GetHowlDataTypeAsString(h),
            1, 
            h.MessageType.ToString(),
            h.Result.ToString()!, uri, payload);
    }
}

// already implemented