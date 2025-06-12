namespace Direwolf.Definitions.Internal.Enums;

/// <summary>
///     The operation to perform on a Direwolf context.
///
/// <remarks>
///     Direwolf uses HTTP-like (if not, the) Transport Protocols, often used to communicate between clients and servers.
///     However, their interpretations inside the Direwolf scope may heavily vary from its HTTP counterpart.
/// </remarks>
/// </summary>
public enum Method
{
    Get,        // read data
    Post,       // update data
    Put,        // add data
    Delete,     // remove data
    Patch
}