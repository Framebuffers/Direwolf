using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.LLM;

namespace Direwolf.Definitions;

/// <summary>
///     A <see cref="Wolfpack" /> is the specialized interchange format of <see cref="Direwolf" />. Any and all communications
///     made, both internally and externally, is made through <see cref="Wolfpack" />s. It is an immutable, value-typed,
///     JsonSchemas-inspired,
///     universal interchange object. <see cref="Wolfpack" />s are identified using <see cref="Cuid" /> (Collision-Resistant
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
/// <param name="MessageResponse">The objective or nature of this message-- what is it asking or what it is about.</param>
/// <param name="Result">The truth value of the request performed: Accepted, Rejected or Cancelled.</param>
/// <param name="DataType">The entry value in this Enum that represents the JsonType of the <see cref="Properties" /> keys,</param>
/// <param name="Properties">Any or all information that has to be transported. Can be null.</param>
/// <param name="JsonSchema">
///     The schema that will be used by <see cref="JsonConverter{T}" /> to serialize/deserialize this
///     <see cref="Wolfpack" />.
/// </param>
public readonly record struct Wolfpack(
    Cuid Id,
    string Name,
    MessageResponse MessageResponse,
    RequestType RequestType,
    ResultType? ResultMessage,
    IDictionary<string, object>? Properties,
    string? Description
)
{
    /// <summary>
    ///     Creates a specialized deep clone of a <see cref="Wolfpack" />, on which only <see cref="Result" />
    ///     and <see cref="Properties" /> are modified.
    /// </summary>
    /// <param name="token">Another token with different Data and ResultType payloads.</param>
    private Wolfpack(Wolfpack token) : this(Cuid.Create(), 
        token.Name, 
        token.MessageResponse,
        token.RequestType, 
        token.ResultMessage, 
        token.Properties,
        token.Description)
    {
    }

    /// <summary>
    ///     <remarks>
    ///         <see cref="MessageResponse" /> and <see cref="Result" /> are null upon creation. They, alongside the
    ///         <see cref="Properties" />,
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
    public static Wolfpack Create(string name, MessageResponse messageResponse, RequestType requestType, IDictionary<string, object>? properties = null, string? description = null)
    {
        var howl = new Wolfpack(Cuid.Create(), name, messageResponse, requestType, null,properties, description);
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
    public Wolfpack DeepCopy(ResultType resultType, Dictionary<string, object> newData)
    {
        return this with { ResultMessage= resultType, Properties = newData, MessageResponse = MessageResponse.Result };
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

    public static CacheItem[]? AsCacheItem(Wolfpack? howl)
    {
        return howl?
            .Properties?
            .Select(payloadElement => 
                new CacheItem(payloadElement.Key, payloadElement.Value))
            .ToArray(); 
    }
    
    
    // To normalize all operations done with Howls, these methods are provided. Use them to make sure
    // you're doing what you want inside the ElementCache.
    
    public static Wolfpack Read(WolfpackParams parameters)
    {
        return Create("read", MessageResponse.Request, RequestType.Get, parameters.Properties!.ToDictionary(x => x.Key, x => x.Value), null);
    }
    
    public static Wolfpack Add(Dictionary<string, object> values)
    {
        return Create("add", MessageResponse.Request, RequestType.Put, values);
    }
    
    public static Wolfpack Delete(WolfpackParams parameters)
    {
        return Create("delete", MessageResponse.Request, RequestType.Delete, parameters.Properties!.ToDictionary(x => x.Key, x => x.Value), null);
    }
    
    public static Wolfpack Update(WolfpackParams parameters)
    {
        return Create("update", MessageResponse.Request, RequestType.Patch, parameters.Properties!.ToDictionary(x => x.Key, x => x.Value), null);
    }
    
}