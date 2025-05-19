using System.Diagnostics;
using System.Globalization;
using System.Runtime.Caching;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;

using Direwolf.Dto;
using Direwolf.Dto.InternalDb.Enums;
using Direwolf.Dto.Parser;
using Direwolf.Dto.RevitApi;
using Direwolf.Extensions;

using Microsoft.Extensions.Caching.Memory;

using Exception = System.Exception;
using MemoryCache = System.Runtime.Caching.MemoryCache;
using Transaction = Direwolf.Dto.InternalDb.Transaction;

namespace Direwolf.Sources.InternalDB;

public class DatabaseChangedEventArgs : EventArgs
{
    public CrudOperation Operation {get; set;}
}

public sealed class Database
{
    public event EventHandler<DatabaseChangedEventArgs> OnDatabaseChanged;

    private static readonly ObjectCache ParameterCache = MemoryCache.Default;
    private static readonly ObjectCache TransactionCache = MemoryCache.Default;
    private static readonly ObjectCache ElementCache = MemoryCache.Default;
    private static readonly ObjectCache RemovedElementsCache = MemoryCache.Default;
    private readonly List<Transaction> _exceptions = [];
    private readonly Stack<WolfDto> _results = new();
    private readonly Queue<WolfDto> _queue = new();
    private readonly Driver? _driver;
    private readonly Document _document;

    public Database(Document document)
    {
        _document = document;
        OnDatabaseChanged += OnDatabaseEvent;
    }

    private void OnDatabaseEvent(object? sender, DatabaseChangedEventArgs e)
    {
        $"Database operation performed: {e.Operation.ToString()}".ToConsole();
    }

    public int GetDatabaseCount()
    {
        return ElementCache.Count();
    }

    public void PopulateDatabase()
    {
        try
        {
            var db = _document.GetRevitDatabase();
            foreach (var r in db)
            {
                ElementCache.Add(new CacheItem(r.Id.ToString(),
                                               r),
                                 null!);
    
            }

            OnDatabaseChanged.Invoke(this, new DatabaseChangedEventArgs { Operation = CrudOperation.Create });
        }
        catch (Exception ex) { ex.LogException(_exceptions); }
    }

    // Transactions = Parameters changed.
    // RevitElement = ID and type data.
    public bool Add(Transaction transaction)
    {
        try
        {
            var newElement = RevitElement.Create(_document,
                                                 transaction.ElementId);
            TransactionCache.Add(new CacheItem(transaction.Id.Value!,
                                               transaction),
                                 null!);
            ElementCache.Add(new CacheItem(newElement.ElementId.ToString()!,
                                           newElement),
                             null!);
            OnDatabaseChanged.Invoke(this,
                                     new DatabaseChangedEventArgs { Operation = CrudOperation.Create });

            return true;
        }
        catch (Exception e)
        {
            e.LogException(_exceptions);

            return false;
        }
    }

    public bool Delete(Transaction transaction)
    {
        try
        {
            RemovedElementsCache.Add(new CacheItem(transaction.Id.Value!,
                                                   transaction),
                                     null!);
            TransactionCache.Remove(transaction.Id.Value!);
            ElementCache.Remove(transaction.ElementId.ToString()!);
            OnDatabaseChanged.Invoke(this,
                                     new DatabaseChangedEventArgs { Operation = CrudOperation.Delete });

            return true;
        }
        catch (Exception e)
        {
            e.LogException(_exceptions);

            return false;
        }
    }

    public bool Update(Transaction transaction)
    {
        try
        {
            Delete(transaction);
            Add(transaction);
            OnDatabaseChanged.Invoke(this,
                                     new DatabaseChangedEventArgs { Operation = CrudOperation.Update });

            return true;
        }
        catch (Exception e)
        {
            e.LogException(_exceptions);

            return false;
        }
    }

    public(IEnumerable<Transaction>? Transactions, RevitElement? ElementData) Read(ElementId id)
    {
        try
        {
            var transactions
                = TransactionCache.GetValues(new List<string> { id.ToString() }).Values.Cast<Transaction>();
            var rvtElement = (RevitElement)ElementCache.Get(id.ToString());
            OnDatabaseChanged.Invoke(this,
                                     new DatabaseChangedEventArgs { Operation = CrudOperation.Read });

            return(transactions, rvtElement);
        }
        catch (Exception e) { e.LogException(_exceptions); }

        return(null, null);
    }

    public IEnumerable<Transaction>? ReadTransactionsOfElement(ElementId id)
    {
        try
        {
            OnDatabaseChanged.Invoke(this,
                                     new DatabaseChangedEventArgs { Operation = CrudOperation.Read });

            return TransactionCache.GetValues(new List<string> { id.ToString() }).Values.Cast<Transaction>();
        }
        catch (Exception e) { e.LogException(_exceptions); }

        return null;
    }
}