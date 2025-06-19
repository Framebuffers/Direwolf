using System.Text.Json.Serialization;

namespace Direwolf.Definitions.LLM;

public readonly record struct McpResourceContainer(
    [property: JsonPropertyName("type")] string JsonType,
    [property: JsonPropertyName("data")] object? Data)
{
    public static McpResourceContainer Create(Howl h)
    {
        return Howl.AsPayload(h);
    }

    public static McpResourceContainer[] Create(Howl[] howl)
    {
        return howl.Select(Howl.AsPayload).ToArray();
    }
}