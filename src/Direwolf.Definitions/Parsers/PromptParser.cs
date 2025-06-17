using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Direwolf.Definitions.Internal.Enums;
using Org.BouncyCastle.Crypto.Generators;

namespace Direwolf.Definitions.Parsers;

[SuppressMessage("ReSharper", "StringIndexOfIsCultureSpecific.1")]
[SuppressMessage("ReSharper", "StringIndexOfIsCultureSpecific.2")]
public readonly record struct WolfpackUri([property: JsonPropertyName("uri")]string Uri)
{
    public readonly string GetProtocol() => Chop(Uri).Protocol;
    public readonly string GetHost() => Chop(Uri).Host;
    public readonly string GetPath() => Chop(Uri).Path;

    public WolfpackUri Create(string uri)
    {
        return new WolfpackUri(uri);
    }

    private (string Protocol, string Host, string Path) Chop(string uri)
    {
        var protocolEnd = uri.IndexOf("://");
        var protocol = uri.Substring(0, protocolEnd);
        var afterProtocol = uri.Substring(protocolEnd + 3);
        var pathStart = afterProtocol.IndexOf('/');
        var host = afterProtocol.Substring(0, pathStart);
        var path = afterProtocol.Substring(pathStart);
        return (protocol, host, path);
    }
}

public readonly record struct WolfpackArguments(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("required")] bool Required,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("direwolf")] int Version,
    [property: JsonPropertyName("message_type")] string MessageType,
    [property: JsonPropertyName("result")] string Result,
    [property: JsonPropertyName("uri")] string Uri)
{
    public static WolfpackArguments Create(Howl h, string uri)
    {
        var cuid = Parsers.Cuid.Create().Value;
        return new WolfpackArguments(
            h.Name ?? null!, h.Description ?? null!, false, Howl.GetHowlDataTypeAsString(h), 1, h.MessageType.ToString(),
            h.Result.ToString()!, uri);
    }
}



public readonly record struct PromptPayload(
    [property: JsonPropertyName("type")] string JsonType,
    [property: JsonPropertyName("data")] object? Data)
{
    public static PromptPayload Create(Howl h)
    {
        return Howl.AsPayload(h);
    }

    public static PromptPayload[] Create(Howl[] howl)
    {
        return howl.Select(Howl.AsPayload).ToArray();
    }
}

// already implemented
public readonly record struct PromptMessage(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")]
    PromptPayload? Content)
{
    public static PromptMessage Create(string role, PromptPayload? content)
    {
        return new PromptMessage(Cuid.Create().Value!, role, content);
    }
}

public readonly record struct PromptResult(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("description")]
    string? Description,
    [property: JsonPropertyName("messages")]
    ImmutableArray<PromptMessage>? Messages,
    [property: JsonIgnore] PromptPayload? Result)
{
    public static PromptResult Create(string role, string? description = null, PromptPayload? results = null, PromptMessage[]? messages = null)
    {
        return new PromptResult(Cuid.Create().Value!, description, messages?.ToImmutableArray(), results);
    }
};