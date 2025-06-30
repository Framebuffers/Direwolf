using System.Text.Json.Serialization;

namespace Direwolf.Definitions.LLM;

public readonly record struct McpResponse(
    [property: JsonPropertyName("id")] Cuid? Id,
    [property: JsonPropertyName("error")] string? Error = null,
    [property: JsonPropertyName("result")] object? Result = null)
{
    public static McpResponse Create(
        object? result, 
        string? error)
    {
        return new McpResponse(Cuid.Create(), error, result);
    }
};