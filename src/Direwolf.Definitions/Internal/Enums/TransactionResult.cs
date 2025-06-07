namespace Direwolf.Definitions.Internal.Enums;

/// <summary>
///     Defines the return state of any given transaction. It mirrors Revit's
///     internal Result enum, but includes
///     an additional value: <see cref="TransactionResult.ExceptionThrown" />. It
///     helps Direwolf process the failure
///     state of any given query instead of just assuming the operation failed and
///     carry on.
/// </summary>
public enum TransactionResult
{
    Success, // operation has been carried out as explicitly instructed
    Failed, // operation failed to return the query as explicitly instructed by the logic operator
    Canceled, // operation has been interrupted at any point by the user

    ExceptionThrown // operation has encountered any kind of exception while it was executing.
    // any transaction that has thrown an Exception *must* be returned inside the Payload field
    // and have its DataType set to RevitException.
}