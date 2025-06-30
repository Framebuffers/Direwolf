namespace Direwolf.Definitions.Enums;

/// <summary>
///     The status of a query or transaction inside Direwolf.
///     <remarks>
///         Conditional logic inside Direwolf will always default to an Error/Exception/False state, unless
///         the control flow changes the state of any given operation.
///         Therefore, ResultType is a MessageResponse where it has been executed, it has changed the truth value to True,
///         and it has a payload.
///     </remarks>
/// </summary>
public enum MessageResponse
{
    Error,          // Request failed.
    Request,        // Expecting a response
    Result,         // Successful response.
    Notification    // Message without a response, one-way.
}