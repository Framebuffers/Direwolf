using System.Text.Json.Serialization;

namespace Direwolf.Definitions.LLM;

public readonly record struct McpResource(
    [property: JsonPropertyName("id")] Cuid? Id,
    [property: JsonPropertyName("uri")] string? Uri,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("mimeType")] string? MimeType)
{
    public static McpResource Create(string? name, string? description, string? uri, string? mimeType)
    {
        return new McpResource(Cuid.Create(), name, description, uri, mimeType);
    }
}