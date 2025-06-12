using System.Text.Json.Serialization;
using Direwolf.Definitions.Parsers;

namespace Direwolf.Definitions.Internal.Enums;

/// <summary>
/// A <see cref="Howl"/> is the specialized interchange format of <see cref="Direwolf"/>. Any and all communications
/// made, both internally and externally, is made through <see cref="Howl"/>s. It is an immutable, value-typed, JSON-inspired,
/// universal interchange object. <see cref="Howl"/>s are identified using <see cref="Cuid"/> (Collision-Resistant
/// Unique Identifiers) to identify themselves. This format encodes both time and date, origin, and ensures a certain
/// level of uniqueness and security. See <see cref="Cuid"/> for more details.
/// <remarks>
/// This format loosely follows the draft specification for <see href="https://modelcontextprotocol.io/introduction">Model Control Protocol</see>
/// by Anthropic. It is designed to also serve as an interoperability object to communicate both internally and
/// externally to any kind of data warehouse; including direct communication between the Direwolf instance and an LLM.
///
/// <see cref="Direwolf"/>, by design, is designed to be platform-agnostic. Regardless of host, any communication
/// to and from the client.
///
/// MCP is implemented as an abstract concept of information exchange between any kind of entity, as it includes
/// all the information needed to comply with the <see href="https://www.jsonrpc.org/specification">JSON-RPC 2.0</see>
/// standard for transport across Clients and Servers.
/// </remarks>
/// 
/// </summary>
/// <param name="Response">The objective or nature of this message-- what is it asking or what it is about.</param>
/// <param name="Result">The truth value of the request performed: Accepted, Rejected or Cancelled.</param>
/// <param name="DataType">The entry value in this Enum that represents the Type of the <see cref="Payload"/> keys,</param>
/// <param name="Payload">Any or all information that has to be transported. Can be null.</param>
/// <param name="JsonSchema">The schema that will be used by <see cref="JsonConverter{T}"/> to serialize/deserialize this <see cref="Howl"/>.</param> 
public readonly record struct Howl(
    Cuid Id,
    Response Response,
    Result? Result,
    DataType DataType,
    Method Method,
    Dictionary<PayloadId, object>? Payload,
    string? Description,
    string JsonSchema)
{
    /// <summary>
    /// Creates a specialized deep clone of a <see cref="Howl"/>, on which only <see cref="Result"/>
    /// and <see cref="Payload"/> are modified.
    /// </summary>
    /// <param name="token">Another token with different Payload and Result payloads.</param>
    private Howl(Howl token) : this(Cuid.Create(), token.Response, token.Result, token.DataType, token.Method, token.Payload,
        token.Description, token.JsonSchema)
    {
    }

    /// <summary>
    /// <remarks>
    /// <see cref="Response"/> and <see cref="Result"/> are null upon creation. They, alongside the <see cref="Payload"/>,
    /// must be assigned a value by using the <see cref="ShallowCopy"/> method.
    /// </remarks>
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="destinationOfData"></param>
    /// <param name="payload"></param>
    /// <param name="jsonSchema"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    public static Howl Create(DataType dt, Method destinationOfData, Dictionary<PayloadId, object> payload, string jsonSchema,
        string? description = null)
    {
        var howl = new Howl(Cuid.Create(), Response: Enums.Response.Request, Result: Enums.Result.Rejected, dt, destinationOfData, payload, description, jsonSchema);
        return howl;
    }

    /// <summary>
    ///  Creates a shallow copy of this <see cref="Howl"/>.
    ///
    /// A shallow copy returns the exact same object, with the exact same <see cref="Id"/>.
    ///
    /// <remarks>
    /// Any changes must be done using the [with] keyword. As with any value-type entity, it will not edit its
    /// parent when modified.
    /// </remarks>
    /// 
    /// </summary>
    /// <returns></returns>
    public Howl ShallowCopy()
    {
        return new Howl(this) { Id = Id };
    }

    /// <summary>
    /// Creates a deep copy of this <see cref="Howl"/>.
    ///
    /// In the <see cref="Direwolf"/> context, a deep-copy is defined as returning the same object, with a
    /// different <see cref="Payload"/>, returning the same <see cref="DataType"/>.
    ///
    /// <remarks>
    /// It's used to manage requests and results inside <see cref="Direwolf"/>. Being value-typed, this object
    /// is bit-by-bit copy of the original, with their respective changes. Like any deep copy, modifying this value
    /// does not modify the original.
    /// </remarks>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="newData"></param>
    /// <returns></returns>
    public Howl DeepCopy(Result result, Dictionary<PayloadId, object> newData)
    {
        return this with { Result = result, Payload = newData, Response = Response.Result};
    }
}