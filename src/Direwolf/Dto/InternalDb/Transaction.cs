using Autodesk.Revit.DB;

using Direwolf.Dto.InternalDb.Enums;
using Direwolf.Dto.Parser;

namespace Direwolf.Dto.InternalDb;

/// <summary>
///     A <see cref="Transaction" /> is the most essential type inside Direwolf. It defines: the operation being performed,
///     which is the data type being returned, and the data blob.
///     <remarks>
///         Transactions are not strongly-typed because of the nature of Revit having several kinds of return values,
///         as well as the kinds of data types that can be returned on a given query. Direwolf will cast the result
///         back to its type by checking the <see cref="DataType" /> value.
///     </remarks>
/// </summary>
/// <param name="Operation">The type of Operation: Create, Read, Update, Delete.</param>
/// <param name="DataType">The expected return type of the value.</param>
/// <param name="Data">A generic object containing the result.</param>
public readonly record struct Transaction(
    Cuid              Id,
    ElementId         ElementId,
    CrudOperation     Operation,
    DataType          DataType,
    TransactionResult Result)
{
    public object Data {get; init;}
}