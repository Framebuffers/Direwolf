using System.Text.Json.Serialization;

namespace Direwolf.Definitions.LLM;

/// <summary>
/// An MCP Resource is any kind of object that an LLM consumes to generate a response. These can be images, documents,
/// API responses, etc.
/// </summary>
/// <param name="Id"></param>
/// <param name="Uri"></param>
/// <param name="Name"></param>
/// <param name="Description"></param>
/// <param name="MimeType"></param>
/// <param name="Data"></param>
public readonly record struct McpResource(
    [property: JsonPropertyName("id")] Cuid? Id,
    [property: JsonPropertyName("uri")] string? Uri,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("mimeType")] string? MimeType,
    [property: JsonPropertyName("data")] object? Data)
{
    public static McpResource Create(string? name, string? description, string? uri, string? mimeType, object? data)
    {
        return new McpResource(Cuid.Create(), uri, name, description, mimeType, data);
    }
}