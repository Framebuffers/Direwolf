using System.Diagnostics;
using System.Globalization;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;

using Direwolf.Dto;
using Direwolf.Dto.InternalDb.Enums;
using Direwolf.Dto.Parser;
using Direwolf.Dto.RevitApi;
using Direwolf.EventArgs;
using Direwolf.Extensions;

using Microsoft.Extensions.Caching.Memory;

using Exception = System.Exception;
using MemoryCache = System.Runtime.Caching.MemoryCache;
using Transaction = Direwolf.Dto.InternalDb.Transaction;

namespace Direwolf.Sources.InternalDB;

public sealed class Database
{
    public event EventHandler<DatabaseChangedEventArgs> DatabaseChangedEventHandler;

    public delegate Database DatabaseProvider();

    public static event DatabaseProvider? OnRequestingDatabase;
    internal static readonly ObjectCache ElementCache = MemoryCache.Default;
    private readonly List<Transaction> _exceptions = [];
    private readonly Stack<WolfDto> _results = new();
    private readonly Queue<WolfDto> _queue = new();
    private readonly Driver? _driver;
    internal readonly Document _document;
    private Database _instance;

    public Database(Document document)
    {
        _document = document;
        DatabaseChangedEventHandler += DatabaseEvent;
        _instance = this;
    }

    public static Database? GetDatabase()
    {
        return OnRequestingDatabase?.Invoke();
    }

    private void DatabaseEvent(object? sender, DatabaseChangedEventArgs e)
    {
        $"Database operation performed: {e.Operation.ToString()}".ToConsole();
    }

    /// <summary>
    /// The Direwolf database schema is a flat, linear structure where <see cref="Autodesk.Revit.DB.ElementId.Value"/>
    /// as a string is the key, and <see cref="RevitElement"/> is the value.
    ///
    /// When the document is fully loaded, Direwolf will populate this database with a record of each valid element.
    /// The result is stored in <see cref="ElementCache"/>. Each subsequent operation is performed over those caches.
    ///
    /// <remarks>
    /// When the cache has to be built from scratch,
    /// use this schema to flush and rebuild the in-memory DB.
    /// </remarks>
    /// </summary>
    public void PopulateDatabase()
    {
        try
        {
            var db = _document.GetRevitDatabase();
            foreach (var r in db)
                ElementCache.Add(new CacheItem(r.ElementId.ToString()!,
                                               r),
                                 null!);

            DatabaseChangedEventHandler.Invoke(this,
                                               new DatabaseChangedEventArgs { Operation = CrudOperation.Create });
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

            ElementCache.Add(new CacheItem(newElement.ElementId.ToString()!,
                                           newElement),
                             null!);
            DatabaseChangedEventHandler.Invoke(this,
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
            ElementCache.Remove(transaction.ElementId.ToString()!);
            DatabaseChangedEventHandler.Invoke(this,
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
            DatabaseChangedEventHandler.Invoke(this,
                                               new DatabaseChangedEventArgs { Operation = CrudOperation.Update });

            return true;
        }
        catch (Exception e)
        {
            e.LogException(_exceptions);

            return false;
        }
    }

    public RevitElement? Read(ElementId id)
    {
        try
        {
            var rvtElement = (RevitElement)ElementCache.Get(id.ToString());
            DatabaseChangedEventHandler.Invoke(this,
                                               new DatabaseChangedEventArgs { Operation = CrudOperation.Read });

            return rvtElement;
        }
        catch (Exception e) { e.LogException(_exceptions); }

        return null;
    }
}