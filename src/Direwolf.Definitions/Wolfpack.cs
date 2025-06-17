using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Autodesk.Revit.UI;
using Direwolf.Definitions.Internal;
using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.Parsers;

namespace Direwolf.Definitions;

public readonly record struct Wolfpack(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonIgnore] RequestType? Method,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("arguments")]
    WolfpackArguments Arguments,
    [property: JsonPropertyName("data")] PromptPayload[]? Data,
    [property: JsonPropertyName("description")]
    string? Description)
{
    public static Wolfpack Create(Cuid? id, RequestType requestType, string? name,
        WolfpackArguments arguments, PromptPayload[] data, string? description = null)
    {
        return new Wolfpack(id?.Value!, requestType, name, arguments, data, description);
    }

    public static Wolfpack Create(RequestType requestType, string? name, WolfpackArguments arguments,
        PromptPayload[] data, string? description = null)
    {
        return new Wolfpack(Cuid.Create().Value!, requestType, name, arguments, data, description);
    }

    public static Wolfpack Get(string? name, WolfpackArguments arguments, out PromptPayload[]? data,
        string? description = null)
    {
        data = null;
        return new Wolfpack(Cuid.Create().Value!, RequestType.Get, name, arguments, null, description);
    }

    public static Wolfpack AsPrompt(Wolfpack w, string promptName, string promptDescription, string uri)
    {
        var args = new WolfpackArguments(promptName, promptDescription, false, "text", 1, MessageType.Result.ToString(), ResultType.Accepted.ToString(), uri);
        var wp = w with { Id = Cuid.Create().Value!, Arguments = args};
        return wp;
    }
}