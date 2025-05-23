using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Transactions;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

using Direwolf.Database.EventArgs;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.Internal;
using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.Parsing;
using Direwolf.Definitions.Revit;

using Transaction = Direwolf.Definitions.Internal.Transaction;

namespace Direwolf.Database;

public readonly record struct OperationRecord(
    UndoOperation          Operation,
    IEnumerable<ElementId> Elements);

public sealed class Engine
{
    public delegate void TransactionHandler(CrudOperation operation);

    public delegate void ResultsHandler(TransactionResult result);

    public event TransactionHandler OnTransactionRequested;
    public event ResultsHandler     OnTransactionExecuted;
    private static readonly object Lock = new object();
    private static Engine? _instance;
    private readonly MemoryCache _transactionCache = MemoryCache.Default;
    private readonly MemoryCache _operationCache = MemoryCache.Default;

    private Engine()
    {
        Debug.Print("Initializing Engine");
        OnTransactionExecuted += result =>
            Debug.Print("Transaction executed: " + result);
        OnTransactionRequested += result =>
            Debug.Print("Transaction executed: " + result);
    }

    public static Engine GetEngine()
    {
        if (_instance is not null)
            return _instance;
        lock (Lock) { _instance ??= new Engine(); }

        return _instance;
    }

    public void GetCount() => _transactionCache.GetCount();

    public void PopulateDatabase(in Document doc)
    {
        OnTransactionRequested.Invoke(CrudOperation.Create);
        var db = doc.GetRevitDatabase();
        foreach (var e in db)
        {
            if (e.ElementId is null
                || e.ElementId.Equals(ElementId.InvalidElementId))
                continue;
            _transactionCache.Add(new CacheItem(e.ElementId!.Value.ToString(),
                                                e),
                                  new CacheItemPolicy()
                                  {
                                      AbsoluteExpiration
                                          = DateTimeOffset.Now.AddHours(1)
                                  });
        }

        OnTransactionExecuted.Invoke(TransactionResult.Success);
    }

    public void Execute(
        CrudOperation     op,
        RevitElement      e,
        out RevitElement? element)
    {
        if (e.ElementId is null
            || e.ElementId.Equals(ElementId.InvalidElementId))
        {
            element = null;

            return;
        }

        switch (op)
        {
            case CrudOperation.Create:
                OnTransactionRequested.Invoke(CrudOperation.Create);
                element = RevitElement.Create(e.Document,
                                              e.ElementId);
                _transactionCache.Add(new CacheItem(
                                          e.ElementId.Value.ToString(),
                                          element),
                                      new CacheItemPolicy()
                                      {
                                          SlidingExpiration
                                              = TimeSpan.FromHours(1)
                                      });

                break;
            case CrudOperation.Read:
                OnTransactionRequested.Invoke(CrudOperation.Read);
                element = (RevitElement)_transactionCache.Get(
                    e.Id.Value!.ToString());

                break;
            case CrudOperation.Update:
                OnTransactionRequested.Invoke(CrudOperation.Update);
                _transactionCache.Remove(e.Id.Value!.ToString());
                element = RevitElement.Create(e.Document,
                                              e.ElementId);
                _transactionCache.Add(new CacheItem(e.Id.Value.ToString(),
                                                    element),
                                      new CacheItemPolicy()
                                      {
                                          SlidingExpiration
                                              = TimeSpan.FromHours(1)
                                      });

                break;
            case CrudOperation.Delete:
                OnTransactionRequested.Invoke(CrudOperation.Delete);
                _transactionCache.Remove(e.ElementId.Value.ToString());

                break;
            default:
                OnTransactionExecuted.Invoke(TransactionResult.Failed);

                throw new ArgumentOutOfRangeException(nameof(op),
                    op,
                    null);
        }

        OnTransactionExecuted.Invoke(TransactionResult.Success);
        element = null;
    }

    public void Execute(
        CrudOperation                  op,
        ICollection<RevitElement>      e,
        out ICollection<RevitElement>? elements)
    {
        switch (op)
        {
            case CrudOperation.Create:
                OnTransactionRequested.Invoke(CrudOperation.Create);
                var creatingResults = new List<RevitElement>();
                foreach (var element in e)
                {
                    var newElement = RevitElement.Create(element.Document,
                        element.ElementId!);
                    _transactionCache.Add(new CacheItem(
                                              newElement.ElementId!.Value
                                                  .ToString(),
                                              element),
                                          new CacheItemPolicy()
                                          {
                                              SlidingExpiration
                                                  = TimeSpan.FromHours(1)
                                          });
                    creatingResults.Add(newElement);
                }

                elements = creatingResults;

                break;
            case CrudOperation.Read:
                OnTransactionRequested.Invoke(CrudOperation.Read);
                var readingResults
                    = e.Select(element =>
                                   (RevitElement)_transactionCache.Get(
                                       element.ElementId!.Value.ToString()))
                       .ToList();
                elements = readingResults;

                break;
            case CrudOperation.Update:
                OnTransactionRequested.Invoke(CrudOperation.Update);
                var updatingResults = new List<RevitElement>();
                foreach (var element in e)
                {
                    _transactionCache.Remove(
                        element.ElementId!.Value.ToString());
                    var newElement = RevitElement.Create(element.Document,
                        element.ElementId!);
                    _transactionCache.Add(new CacheItem(
                                              newElement.ElementId!.Value
                                                  .ToString(),
                                              element),
                                          new CacheItemPolicy()
                                          {
                                              SlidingExpiration
                                                  = TimeSpan.FromHours(1)
                                          });
                    updatingResults.Add(newElement);
                }

                elements = updatingResults;

                break;
            case CrudOperation.Delete:
                OnTransactionRequested.Invoke(CrudOperation.Delete);
                foreach (var element in e)
                {
                    _transactionCache.Remove(
                        element.ElementId!.Value!.ToString());
                }

                elements = null;

                break;
            default:
                OnTransactionExecuted.Invoke(TransactionResult.Failed);

                throw new ArgumentOutOfRangeException(nameof(op),
                    op,
                    null);
        }

        elements = null;
        OnTransactionExecuted.Invoke(TransactionResult.Success);
    }

    public void DropDocument(Document doc)
    {
        OnTransactionRequested.Invoke(CrudOperation.Delete);
        foreach (string? elementToDelete in
                 from cacheItem in _transactionCache
                 let key = cacheItem.Key
                 let value = (RevitElement)cacheItem.Value
                 where value.Document.Equals(doc)
                 select key) { _transactionCache.Remove(elementToDelete); }

        OnTransactionExecuted.Invoke(TransactionResult.Success);
    }

    public IEnumerable<RevitElement> GetCachedElements(Document doc)
    {
        return from e in _transactionCache
               let rvtElement = (RevitElement)e.Value
               where rvtElement.Document.Equals(doc)
               select rvtElement;
    }

    public void DisposeDatabase() => _transactionCache.Dispose();
}