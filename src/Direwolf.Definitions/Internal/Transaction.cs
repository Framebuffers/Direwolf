using System.Runtime.Caching;
using Autodesk.Revit.DB;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.Parser;

namespace Direwolf.Definitions.Internal;

/// <summary>
///     A <see cref="Transaction" /> is the most essential type inside Direwolf. It
///     defines: the operation being performed,
///     which is the data type being returned, and the data blob.
///     <remarks>
///         Transactions are not strongly-typed because of the nature of Revit
///         having several kinds of return values,
///         as well as the kinds of data types that can be returned on a given
///         query. Direwolf will cast the result
///         back to its type by checking the <see cref="DataType" /> value.
///     </remarks>
/// </summary>
/// <param name="Operation">The type of Operation: CreateRevitId, Read, Update, Delete.</param>
/// <param name="DataType">The expected return type of the value.</param>
public readonly record struct Transaction(
    Cuid Id,
    ElementId ElementId,
    string ElementUniqueId,
    CrudOperation Operation,
    DataType DataType,
    TransactionResult Result)
{
    public object? Payload { get; init; }

    /// <summary>
    ///     Defines, identifies and details an operation to be performed on <see cref="Direwolf" /> ElementCache.
    ///     This record holds identification and operation data to CreateRevitId, Read, Update or Delete data from
    ///     <see cref="Direwolf" />.
    ///     TransactionCache.
    ///     A <see cref="Transaction" /> is an immutable record struct, therefore, any updates to it are returned as a shallow
    ///     copy of the original.
    /// </summary>
    /// <param name="elementId">The ID of the Element being used to perform the Transaction on.</param>
    /// <param name="Document">The Revit Document of which the Transaction originated from.</param>
    /// <param name="op">The Operation to be performed inside Direwolf.</param>
    /// <param name="type">The Type of the information being held inside the <see cref="Payload" /> field.</param>
    /// <param name="data">
    ///     A payload field to be used to pass any information required by, or returned from the execution of
    ///     this <see cref="Transaction" />.
    /// </param>
    /// <returns>A <see cref="CacheItem" /> to be used inside <see cref="Direwolf" /> TransactionCache.</returns>
    public static Transaction Create(
        ElementId elementId, Document doc, CrudOperation op, DataType type, object? data = null)
    {
        var docId = (doc.GetDocumentVersionCounter(), doc.GetDocumentUuidHash());
        return new Transaction(Cuid.CreateRevitId(doc, out docId),
            elementId,
            doc.GetElement
                    (elementId)
                .UniqueId,
            op,
            type,
            TransactionResult.Success) { Payload = data };
    }

    /// <summary>
    ///     Creates a new Transaction formatted as a <see cref="CacheItem" /> to be used inside the <see cref="Direwolf" />
    ///     TransactionCache.
    /// </summary>
    /// <param name="elementId">The ID of the Element being used to perform the Transaction on.</param>
    /// <param name="doc">The Revit Document of which the Transaction originated from.</param>
    /// <param name="op">The Operation to be performed inside Direwolf.</param>
    /// <param name="type">The Type of the information being held inside the <see cref="Payload" /> field.</param>
    /// <param name="data">
    ///     A payload field to be used to pass any information required by, or returned from the execution of
    ///     this <see cref="Transaction" />.
    /// </param>
    /// <returns>A <see cref="CacheItem" /> to be used inside <see cref="Direwolf" /> TransactionCache.</returns>
    public static CacheItem CreateAsCacheItem(
        ElementId elementId, Document doc, CrudOperation op, DataType type, object? data = null)
    {
        var transaction = Create
        (elementId,
            doc,
            op,
            type,
            data);

        return new CacheItem(transaction.Id.Value,
            // ReSharper disable once HeapView.BoxingAllocation
            transaction);
    }

    /// <summary>
    ///     Creates a copy of a Transaction with the same <see cref="ElementId" />,
    ///     but with an updated <see cref="Payload" /> property. A new <see cref="Id" />
    ///     is issued.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Transaction ShallowCopy(in Transaction t)
    {
        return t with
        {
            Id = Cuid.Create(),
            Payload = t.Payload
        };
    }

    /// <summary>
    ///     Returns this object as a <see cref="CacheItem" /> correctly formatted to be consistently trackable inside
    ///     the <see cref="Direwolf" /> TransactionCache context.
    /// </summary>
    /// <returns>A CacheItem ready to be used inside the <see cref="Direwolf" /> Transaction </returns>
    public CacheItem AsCacheItem()
    {
        return new CacheItem(Id.Value,
            this);
    }
}