using System.Text.Json.Serialization;

namespace Direwolf.Definitions.LLM;

public record McpError(
    [property: JsonPropertyName("code")] int ErrorCode,
    [property: JsonPropertyName("message")] string? ErrorMessage,
    [property: JsonPropertyName("data")] object? ErrorData);