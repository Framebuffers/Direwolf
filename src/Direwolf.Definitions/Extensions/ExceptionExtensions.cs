using Autodesk.Revit.DB;
using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.Parser;
using Transaction = Direwolf.Definitions.Internal.Transaction;

namespace Direwolf.Definitions.Extensions;

/// <summary>
///     Helper methods for <see cref="Exception" /> inside the Direwolf context.
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    ///     Creates a special <see cref="Transaction" /> that holds Exception data thrown whenever a
    ///     <see cref="CrudOperation" /> has failed.
    /// </summary>
    /// <param name="ex">Exception being thrown.</param>
    /// <param name="exceptionList">The list where the Exception Transaction list is being held.</param>
    public static void LogException(this Exception ex, List<Transaction> exceptionList)
    {
        switch (ex?.GetType()
                    ?.Namespace?.Contains
                        ("Autodesk.Revit"))
        {
            case false:
                exceptionList.Add
                (new Transaction(new Cuid(),
                    ElementId.InvalidElementId,
                    null!,
                    CrudOperation.Create,
                    DataType.SystemException,
                    TransactionResult.ExceptionThrown) { Payload = ex });

                break;
            default:
                if (ex is null) break;
                exceptionList.Add
                (new Transaction(new Cuid(),
                    ElementId.InvalidElementId,
                    null!,
                    CrudOperation.Create,
                    DataType.RevitException,
                    TransactionResult.ExceptionThrown) { Payload = ex });

                break;
        }
    }
}