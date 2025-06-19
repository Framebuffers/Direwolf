using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Direwolf.Driver.MCP;

/// <summary>
/// Converts a URI string into a Wolfpack record, with facilities to manipulate its components.
///
/// <remarks>
///     The Wolfpack URI schema follows this nomenclature:
/// <para></para>
///         wolfpack://[developer hostname, in reverse domain format]-[version tag]/[hierarchical]/[path]/[to]/[object]?[an operation identifying character]=[operation, without spaces, as lower]/
/// <para>where [operation identifying character] can be:</para>
///     <list>
///         <item>p = parameter</item>
///         <item>q = query</item>
///         <item>r = prompt</item>
///         <item>s = notification</item>
///     </list>
/// <para>  where the hostname's reverse domain schema follows this example:</para>
/// <para>  wolfpack://com.appname.manufacturer-version/</para>
/// <para>  wolfpack://org.standariztionbody-standard.name/</para>
/// <para>  wolfpack://std.standardname-version/</para>
///         where <b>std</b> is shorthand for standard.
/// <para></para>
/// The reverse domain name follows both a schema common in other programming languages (like Java) for familiarity,
/// as well avoiding creating hostnames that sound like domains.
/// It is encouraged for developers to use this schema to standardize access to hierarchies for each developer or standard. 
/// </remarks>
/// </summary>
/// <param name="Uri"></param>
[SuppressMessage("ReSharper", "StringIndexOfIsCultureSpecific.1")]
[SuppressMessage("ReSharper", "StringIndexOfIsCultureSpecific.2")]
internal readonly record struct WolfpackUri([property: JsonPropertyName("uri")]string Uri)
{
    public string GetProtocol() => Chop(Uri).Protocol;
    public string GetHost() => Chop(Uri).Host;
    public string GetPath() => Chop(Uri).Path;

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