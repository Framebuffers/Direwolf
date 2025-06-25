using System.Runtime.Caching;
using System.Text.Json.Serialization;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.LLM;

namespace Direwolf.Definitions;

// /// <summary>
// ///     A <see cref="Wolfpack" /> is the specialized interchange format of <see cref="Direwolf" />. Any and all communications
// ///     made, both internally and externally, is made through <see cref="Wolfpack" />s. It is an immutable, value-typed,
// ///     JsonSchemas-inspired,
// ///     universal interchange object. <see cref="Wolfpack" />s are identified using <see cref="Cuid" /> (Collision-Resistant
// ///     Unique Identifiers) to identify themselves. This format encodes both time and date, origin, and ensures a certain
// ///     level of uniqueness and security. See <see cref="Cuid" /> for more details.
// ///     <remarks>
// ///         This format loosely follows the draft specification for
// ///         <see href="https://modelcontextprotocol.io/introduction">Model Control Protocol</see>
// ///         by Anthropic. It is designed to also serve as an interoperability object to communicate both internally and
// ///         externally to any kind of data warehouse; including direct communication between the Direwolf instance and an
// ///         LLM.
// ///         <see cref="Direwolf" />, by design, is designed to be platform-agnostic. Regardless of host, any communication
// ///         to and from the client.
// ///         MCP is implemented as an abstract concept of information exchange between any kind of entity, as it includes
// ///         all the information needed to comply with the
// ///         <see href="https://www.jsonrpc.org/specification">JsonSchemas-RPC 2.0</see>
// ///         standard for transport across Clients and Servers.
// ///     </remarks>
// /// </summary>
// /// <Properties>
// /// 
// /// </Properties>

/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="MessageResponse"></param>
/// <param name="RequestType"></param>
/// <param name="Result"></param>
/// <param name="Data"></param>
/// <param name="Description"></param>
public readonly record struct Wolfpack(
    [property: JsonIgnore] Cuid Id,
    [property: JsonPropertyName("name"), JsonPropertyOrder(1)]string Name,
    [property: JsonIgnore]MessageResponse MessageResponse,
    [property: JsonIgnore]RequestType RequestType,
    [property: JsonPropertyName("result")]object? Result,
    [property: JsonPropertyName("parameters"), JsonPropertyOrder(3)]object? Parameters,
    [property: JsonPropertyName("description"), JsonPropertyOrder(2)]string? Description
)
{
    [JsonPropertyName("id"), JsonPropertyOrder(0)]
    public readonly string SerializableId = Id.ToString();
    [JsonPropertyName("jsonrpc"), JsonPropertyOrder(0)] public const string JsonRpc = "2.0";
    [JsonPropertyName("wolfpack")] public const string WolfpackVersion = "1.0";
    [JsonPropertyName("createdAt")] public DateTimeOffset? CreatedAt => Id.GetDateTimeCreation();
    [JsonPropertyName("updatedAt")] public DateTime UpdatedAt => DateTime.UtcNow;
    
    /// <summary>
    ///     Creates a specialized deep clone of a <see cref="Wolfpack" />, on which only <see cref="Result" />
    ///     and <see cref="Data" /> are modified.
    /// </summary>
    /// <param name="token">Another token with different Properties and ResultType payloads.</param>
    private Wolfpack(Wolfpack token) : this(Cuid.Create(), 
        token.Name, 
        token.MessageResponse,
        token.RequestType, 
        token.Result, 
        token.Parameters,
        token.Description)
    {
    }

    /// <summary>
    ///     <remarks>
    ///         <see cref="MessageResponse" /> and <see cref="Result" /> are null upon creation. They, alongside the
    ///         <see cref="Data" />,
    ///         must be assigned a value by using the <see cref="ShallowCopy" /> requestType.
    ///     </remarks>
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="destinationOfData"></param>
    /// <param name="payload"></param>
    /// <param name="jsonSchema"></param>
    /// <param name="description"></param>
    /// <param name="name">Name for this Wolfpack</param>
    /// <returns></returns>
    public static Wolfpack Create(string name, MessageResponse messageResponse, RequestType requestType, object? result, object? @params, string? description = null)
    {
        var howl = new Wolfpack(Cuid.Create(), name, messageResponse, requestType, result, @params, description);
        return howl;
    }

    /// <summary>
    ///     Creates a shallow copy of this <see cref="Wolfpack" />.
    ///     A shallow copy returns the exact same object, with the exact same <see cref="Id" />.
    ///     <remarks>
    ///         Any changes must be done using the [with] keyword. As with any value-type entity, it will not edit its
    ///         parent when modified.
    ///     </remarks>
    /// </summary>
    /// <returns></returns>
    public Wolfpack ShallowCopy()
    {
        return new Wolfpack(this) { Id = Id };
    }

    /// <summary>
    ///     Creates a deep copy of this <see cref="Wolfpack" />.
    ///     In the <see cref="Direwolf" /> context, a deep-copy is defined as returning the same object, with a
    ///     different <see cref="Data" />, returning the same <see cref="DataType" />.
    ///     <remarks>
    ///         It's used to manage requests and results inside <see cref="Direwolf" />. Being value-typed, this object
    ///         is bit-by-bit copy of the original, with their respective changes. Like any deep copy, modifying this value
    ///         does not modify the original.
    ///     </remarks>
    /// </summary>
    /// <param name="resultType"></param>
    /// <param name="newParams"></param>
    /// <returns></returns>
    public Wolfpack DeepCopy(ResultType resultType, Dictionary<string, object> newParams)
    {
        return this with { Result= resultType, Parameters = newParams, MessageResponse = MessageResponse.Result };
    }

    // public static string GetHowlDataTypeAsString(Wolfpack wolfpack)
    // {
    //     return wolfpack.DataType switch
    //     {
    //         DataType.Null => "null",
    //         DataType.Empty => "null",
    //         DataType.Invalid => "object",
    //         DataType.String => "string",
    //         DataType.Boolean => "bool",
    //         DataType.Numbers => "numbers",
    //         DataType.Double => "numbers",
    //         DataType.FloatingPoint => "numbers",
    //         DataType.Array => "array",
    //         DataType.Object => "object",
    //         _ => "object",
    //     }; 
    // }

    public static CacheItem? AsCacheItem(Wolfpack? howl)
    {
        return new CacheItem(howl?.Id.Value, howl);
    }
}