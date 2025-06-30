using System.Text.Json.Serialization;

namespace Direwolf.Definitions.LLM;

public record McpTool(
    [property: JsonPropertyName("name")] string ToolName,
    [property: JsonPropertyName("description")] string ToolDescription,
    [property: JsonPropertyName("inputSchema")] object ToolInputSchema);