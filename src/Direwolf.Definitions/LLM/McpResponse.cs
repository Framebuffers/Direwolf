using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Direwolf.Definitions.Enums;

namespace Direwolf.Definitions.LLM;

public readonly record struct McpResponse(
    [property: JsonPropertyName("id")] Cuid? Id,
    [property: JsonPropertyName("error")] string? Error,
    [property: JsonPropertyName("result")] ImmutableArray<McpResource>? Result)
{
    public static McpResponse Create(
        McpResource[]? results, 
        string? error)
    {
        return new McpResponse(Cuid.Create(),
            error ?? null,
                results?.ToImmutableArray());
    }
};