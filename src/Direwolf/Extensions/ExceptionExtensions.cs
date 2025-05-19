using Direwolf.Dto.InternalDb;
using Direwolf.Dto.InternalDb.Enums;
using Direwolf.Dto.Parser;

namespace Direwolf.Extensions;

public static class ExceptionExtensions
{
    public static void LogException(this Exception ex, List<Transaction> exceptionList)
    {
        switch (ex?.GetType()?.Namespace?.Contains("Autodesk.Revit"))
        {
            case false:
                exceptionList.Add(new Transaction(new Cuid(),
                                                  null!,
                                                  CrudOperation.Create,
                                                  DataType.SystemException,
                                                  TransactionResult.ExceptionThrown) { Data = ex });

                break;
            default:
                if (ex is null) break;
                exceptionList.Add(new Transaction(new Cuid(),
                                                  null!,
                                                  CrudOperation.Create,
                                                  DataType.RevitException,
                                                  TransactionResult.ExceptionThrown) { Data = ex });

                break;
        }
    }
}