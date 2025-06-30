// using System.Text.Json.Serialization;
//
// namespace Direwolf.Definitions.LLM;
//
// public readonly record struct McpResourceContainer(
//     [property: JsonPropertyName("type")] string JsonType,
//     [property: JsonPropertyName("data")] object? Properties)
// {
//     public static McpResourceContainer Create(Wolfpack h)
//     {
//         return Wolfpack.AsPayload(h);
//     }
//
//     public static McpResourceContainer[] Create(Wolfpack[] howl)
//     {
//         return howl.Select(Wolfpack.AsPayload).ToArray();
//     }
// }