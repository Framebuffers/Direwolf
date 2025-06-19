using System.Runtime.Caching;
using System.Text.Json.Serialization;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.LLM;
using Direwolf.Definitions.Serialization;
using Org.BouncyCastle.Tls;

namespace Direwolf.Definitions;

/// <summary>
///     A <see cref="Howl" /> is the specialized interchange format of <see cref="Direwolf" />. Any and all communications
///     made, both internally and externally, is made through <see cref="Howl" />s. It is an immutable, value-typed,
///     JsonSchemas-inspired,
///     universal interchange object. <see cref="Howl" />s are identified using <see cref="Cuid" /> (Collision-Resistant
///     Unique Identifiers) to identify themselves. This format encodes both time and date, origin, and ensures a certain
///     level of uniqueness and security. See <see cref="Cuid" /> for more details.
///     <remarks>
///         This format loosely follows the draft specification for
///         <see href="https://modelcontextprotocol.io/introduction">Model Control Protocol</see>
///         by Anthropic. It is designed to also serve as an interoperability object to communicate both internally and
///         externally to any kind of data warehouse; including direct communication between the Direwolf instance and an
///         LLM.
///         <see cref="Direwolf" />, by design, is designed to be platform-agnostic. Regardless of host, any communication
///         to and from the client.
///         MCP is implemented as an abstract concept of information exchange between any kind of entity, as it includes
///         all the information needed to comply with the
///         <see href="https://www.jsonrpc.org/specification">JsonSchemas-RPC 2.0</see>
///         standard for transport across Clients and Servers.
///     </remarks>
/// </summary>
/// <param name="MessageType">The objective or nature of this message-- what is it asking or what it is about.</param>
/// <param name="Result">The truth value of the request performed: Accepted, Rejected or Cancelled.</param>
/// <param name="DataType">The entry value in this Enum that represents the JsonType of the <see cref="Properties" /> keys,</param>
/// <param name="Properties">Any or all information that has to be transported. Can be null.</param>
/// <param name="JsonSchema">
///     The schema that will be used by <see cref="JsonConverter{T}" /> to serialize/deserialize this
///     <see cref="Howl" />.
/// </param>
public readonly record struct Howl(
    Cuid Id,
    MessageType MessageType,
    ResultType? Result,
    DataType DataType,
    RequestType RequestType,
    IDictionary<string, object>? Properties,
    string? Description,
    string? Name)
{
    /// <summary>
    ///     Creates a specialized deep clone of a <see cref="Howl" />, on which only <see cref="Result" />
    ///     and <see cref="Properties" /> are modified.
    /// </summary>
    /// <param name="token">Another token with different Data and ResultType payloads.</param>
    private Howl(Howl token) : this(Cuid.Create(), token.MessageType, token.Result, token.DataType, token.RequestType,
        token.Properties,
        token.Description, token.Name)
    {
    }

    /// <summary>
    ///     <remarks>
    ///         <see cref="MessageType" /> and <see cref="Result" /> are null upon creation. They, alongside the
    ///         <see cref="Properties" />,
    ///         must be assigned a value by using the <see cref="ShallowCopy" /> requestType.
    ///     </remarks>
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="destinationOfData"></param>
    /// <param name="payload"></param>
    /// <param name="jsonSchema"></param>
    /// <param name="description"></param>
    /// <param name="name">Name for this Howl</param>
    /// <returns></returns>
    public static Howl Create(DataType dt, RequestType destinationOfData, Dictionary<string, object> payload,
        string? description = null, string? name = null)
    {
        var howl = new Howl(Cuid.Create(), MessageType.Request, ResultType.Rejected, dt, destinationOfData, payload,
            description, name ?? string.Empty);
        return howl;
    }

    /// <summary>
    ///     Creates a shallow copy of this <see cref="Howl" />.
    ///     A shallow copy returns the exact same object, with the exact same <see cref="Id" />.
    ///     <remarks>
    ///         Any changes must be done using the [with] keyword. As with any value-type entity, it will not edit its
    ///         parent when modified.
    ///     </remarks>
    /// </summary>
    /// <returns></returns>
    public Howl ShallowCopy()
    {
        return new Howl(this) { Id = Id };
    }

    /// <summary>
    ///     Creates a deep copy of this <see cref="Howl" />.
    ///     In the <see cref="Direwolf" /> context, a deep-copy is defined as returning the same object, with a
    ///     different <see cref="Properties" />, returning the same <see cref="DataType" />.
    ///     <remarks>
    ///         It's used to manage requests and results inside <see cref="Direwolf" />. Being value-typed, this object
    ///         is bit-by-bit copy of the original, with their respective changes. Like any deep copy, modifying this value
    ///         does not modify the original.
    ///     </remarks>
    /// </summary>
    /// <param name="resultType"></param>
    /// <param name="newData"></param>
    /// <returns></returns>
    public Howl DeepCopy(ResultType resultType, Dictionary<string, object> newData)
    {
        return this with { Result = resultType, Properties = newData, MessageType = MessageType.Result };
    }

    public static string GetHowlDataTypeAsString(Howl howl)
    {
        return howl.DataType switch
        {
            DataType.Null => "null",
            DataType.Empty => "null",
            DataType.Invalid => "object",
            DataType.String => "string",
            DataType.Boolean => "bool",
            DataType.Numbers => "numbers",
            DataType.Double => "numbers",
            DataType.FloatingPoint => "numbers",
            DataType.Array => "array",
            DataType.Object => "object",
            _ => "object",
        }; 
    }

    public static CacheItem[]? AsCacheItem(Howl? howl)
    {
        return howl?
            .Properties?
            .Select(payloadElement => 
                new CacheItem(payloadElement.Key, payloadElement.Value))
            .ToArray(); 
    }
    
    public static McpResourceContainer AsPayload(Howl howl)
    {
        var type = GetHowlDataTypeAsString(howl);
        howl = howl with { Result = ResultType.Accepted };
        return howl.Properties is null ? new McpResourceContainer(type, null) : new McpResourceContainer(type, howl.Properties);
    }
    
    // To normalize all operations done with Howls, these methods are provided. Use them to make sure
    // you're doing what you want inside the ElementCache.
    
    public static Howl Read(string[]? uniqueIds)
    {
              return Create(DataType.Array, RequestType.Get,
                    new Dictionary<string, object>() { ["key"] = uniqueIds! });
    }

    public static Howl Add(Dictionary<string, object> values, string? name, string? description)
    {
        return Create(
            DataType.Array, RequestType.Put, values, description, name);
    }

    public static Howl Delete(string[]? uniqueIds, string? name, string? description)
    {
        return Create(
            DataType.Array, RequestType.Delete, new() {["key"] = uniqueIds!},
            description ?? $"elements: {uniqueIds?.Length}", name ?? "elementDeletion");
    }

    public static Howl Update(string[] uniqueIds, string? name, string? description)
    {
        return Create(
            DataType.Array, RequestType.Post, new() { ["key"] = uniqueIds! }, description,
            name);
    }

    public static (McpResource Metadata, McpResourceContainer Payload) AsMcpData(Howl h, string uri, string mimeType)
    {
        var description =
            $"howl-token\ncuid: {h.Id.Value}\n,type: {h.DataType.ToString()}\nresult{h.Result.ToString()}\ndwDtaType: {h.DataType.ToString()}\ndateTime: {h.Id.GetDateTimeCreation().UtcDateTime}" +
            $"{h.Description}";
        var res = McpResource.Create(
            h.Name ?? nameof(Howl), description, uri, mimeType);
        return (res, new McpResourceContainer("object", h.Properties));
    }
    
}