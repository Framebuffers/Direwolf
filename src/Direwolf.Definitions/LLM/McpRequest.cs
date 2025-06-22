using System.Text.Json.Serialization;

namespace Direwolf.Definitions.LLM;

public record McpRequest(
[property: JsonPropertyName("method"), JsonPropertyOrder(1)] string? Method,
[property: JsonPropertyName("params"), JsonPropertyOrder(2)] object? Params)
{
    [JsonPropertyName("jsonrpc"), JsonPropertyOrder(0)] public const string JsonRpc = "2.0";

    [JsonPropertyName("id"), JsonPropertyOrder(3)]
    public string Id { get; set; } = Cuid.Create().Value!;

    public static McpRequest Create(string method, object? @params)
    {
        return new McpRequest(method, @params);
    }
}