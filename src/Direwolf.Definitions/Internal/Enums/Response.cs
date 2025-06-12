namespace Direwolf.Definitions.Internal.Enums;

/// <summary>
///     The status of a query or transaction inside Direwolf.
///     <remarks>
///         Conditional logic inside Direwolf will always default to an Error/Exception/False state, unless
///         the control flow changes the state of any given operation.
///         Therefore, Result is a Response where it has been executed, it has changed the truth value to True,
///         and it has a payload.
///     </remarks>
/// </summary>
public enum Response
{
    Error, // The operation has not been performed, failed to do so, or has encountered an error.
    Request, // Initial default state of all Responses. Indicates this operation is an unprocessed request.
    Result, // The operation is not of the Error type. See remarks for more info.
    Notification // The operation has been performed, and the request returned a null value. It's like saying OK.
}