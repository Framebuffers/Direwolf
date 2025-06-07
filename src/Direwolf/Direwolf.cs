using System.Diagnostics;
using System.Runtime.Caching;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Direwolf.Definitions.Drivers;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.RevitApi;
using Direwolf.EventArgs;
using Exception = System.Exception;
using MemoryCache = System.Runtime.Caching.MemoryCache;
using Transaction = Direwolf.Definitions.Internal.Transaction;

// ReSharper disable HeapView.ObjectAllocation.Evident
// ReSharper disable HeapView.ClosureAllocation
// ReSharper disable HeapView.DelegateAllocation
// ReSharper disable HeapView.ObjectAllocation.Possible
// ReSharper disable HeapView.ObjectAllocation
namespace Direwolf;

/// <summary>
///     This class holds the entire instance of the Direwolf Engine and DB. It is composed of two caches:
///     <list type="bullet">
///         <item>
///             <term><see cref="ElementCache" />: </term>
///             <description>
///                 Holds a <see cref="RevitElement" /> mirror for every single
///                 <see cref="Autodesk.Revit.DB.Element" /> inside the <see cref="Document" />
///             </description>
///         </item>
///         <item>
///             <term> <see cref="TransactionCache" />: </term>
///             <description>
///                 Holds a registry of all <see cref="Transaction" /> being performed inside the
///                 <see cref="Direwolf" /> context.
///             </description>
///         </item>
///     </list>
///     Direwolf is a thread-safe Singleton. There's a single instance for the whole Revit Application, and it handles
///     multiple requests being performed by different threads at once. Each operation must pass their respective
///     <see cref="Document" />
///     to perform the <see cref="Transaction" /> it requires. Each one is tracked
/// </summary>
public sealed class Direwolf
{
    private static readonly object Lock = new();
    internal static readonly ObjectCache ElementCache = MemoryCache.Default;

    // ReSharper disable once MemberCanBePrivate.Global
    private static ControlledApplication? _controlledApplication;
    private static Direwolf? _instance;
    private static readonly CacheItemPolicy Policy = new() { SlidingExpiration = TimeSpan.FromMinutes(60) };
    private readonly List<Transaction> _exceptions = [];

    private Direwolf(Document document)
    {
        DatabaseChangedEventHandler += DatabaseEvent;
        _instance = this;
        _instance.PopulateDatabase(document);
    }

    public event EventHandler<DatabaseChangedEventArgs>? DatabaseChangedEventHandler;

    public static Direwolf GetDatabase(Document doc)
    {
        if (_instance is not null) return _instance;
        lock (Lock)
        {
            if (_instance is not null) return _instance;
            _instance = new Direwolf(doc);
            return _instance;
        }
    }

    /// <summary>
    ///     Sends a message to the Debug console whenever a <see cref="Transaction" /> has been executed, and the
    ///     state of the <see cref="ElementCache" /> has been altered.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void DatabaseEvent(object? sender, DatabaseChangedEventArgs e)
    {
        Debug.Print($"Direwolf operation performed: {e.Operation}");
    }

    /// <summary>
    ///     The Direwolf database schema is a flat, linear structure where
    ///     <see cref="Autodesk.Revit.DB.ElementId.Value" />
    ///     as a string is the key, and <see cref="RevitElement" /> is the value.
    ///     When the document is fully loaded, Direwolf will populate this database
    ///     with a record of each valid element.
    ///     The result is stored in <see cref="ElementCache" />. Each subsequent
    ///     operation is performed over those caches.
    ///     <remarks>
    ///         When the cache has to be built from scratch,
    ///         use this schema to flush and rebuild the in-memory DB.
    ///     </remarks>
    /// </summary>
    private void PopulateDatabase(Document doc)
    {
        //TODO: add elements as elements without parameters. when a parameter is requested, shallow copy with the parameters attached. 
        try
        {
            foreach (var cacheItem in doc.GetRevitDatabaseAsCacheItems()) ElementCache.Add(cacheItem, Policy);
            DatabaseChangedEventHandler?.Invoke(this,
                new DatabaseChangedEventArgs() { Operation = CrudOperation.Create });
        }
        catch (Exception ex)
        {
            ex.LogException(_exceptions);
        }
    }

    /// <summary>
    ///     Adds a new <see cref="RevitElement" /> to the cache, or updates its value.
    /// </summary>
    /// <param name="elementUniqueId">
    ///     The Transaction containing the ElementId to add, and
    ///     any other data to add.
    /// </param>
    /// <param name="doc"></param>
    /// <returns>True if the operation was completed successfully, false otherwise. </returns>
    public bool Add(string elementUniqueId, Document doc)
    {
        try
        {
            var element = RevitElement.CreateAsCacheItem(doc, elementUniqueId, out _);
            if (element is null) return false;
            
            ElementCache?.Add(element, Policy);
            Debug.Print($"Adding to Document: {doc.GetDocumentUuidHash()}::{doc.Title}");
            return true;
        }
        catch (Exception e)
        {
            e.LogException(_exceptions);
            return false;
        }
    }
    
    //TODO: Whenever an update is triggered, check by the key (Element.UniqueID), match and shallow-copy
    //TODO: Don't know if the lifecycle of the element includes its parameters, or if something else changes, it's destroyed and reissued.
    public bool Update(string elementUniqueId, Document doc)
    {
        try
        {
            if (ElementCache?.Contains(elementUniqueId) is true)
            {
                Debug.Print($"\tUpdating from Document: {doc.GetDocumentUuidHash()}::{doc.Title}");
                var element = (RevitElement)ElementCache.Get(elementUniqueId);
                // ReSharper disable once HeapView.BoxingAllocation
                ElementCache.Add(
                    new CacheItem(element.ElementUniqueId,
                        element with
                        {
                            CategoryType = element.CategoryType,
                            BuiltInCategory = element.BuiltInCategory,
                            ElementId = element.ElementId,
                            Parameters = element.Parameters,
                        }), Policy);
            }
            else
            {
                return false;
            }

            DatabaseChangedEventHandler?.Invoke(this,
                new DatabaseChangedEventArgs() { Operation = CrudOperation.Delete });
            return true;
        }
        catch (Exception e)
        {
            e.LogException(_exceptions);
            return false;
        }
    }

    /// <summary>
    ///     Deletes a <see cref="RevitElement" /> with the same
    ///     <see cref="Autodesk.Revit.DB.ElementId" /> as the one
    ///     inside the <see cref="Transaction" />.
    /// </summary>
    /// <param name="transaction">
    ///     The Transaction containing the ElementId to add, and
    ///     any other data to add.
    /// </param>
    /// <param name="doc">Revit Document</param>
    /// <returns>True if the operation was completed successfully, false otherwise. </returns>
    public bool Delete(string elementUniqueId, Document doc)
    {
        try
        {
            Debug.Print($"\tUpdating from Document: {doc.GetDocumentUuidHash()}::{doc.Title}");
            //
            // // for all elements in the cache
            // foreach (var element in Wolfden)
            // {
            //     // if I accidentally loaded a transaction here, avoid
            //     if (element.Value is not RevitElement r) continue;
            //
            //     // check if the element belongs to that document
            //     if (!RevitElement.BelongsToDocument
            //         (r,
            //             doc))
            //         continue;
            //
            //     // check if the element exists by uniqueID
            //     if (!r.ElementUniqueId.Equals
            //             (transaction.ElementUniqueId))
            //         continue;
            //
            //     // if found, remove itself using the CUID of the value
            //     Wolfden.Remove
            //         (element.Key);
            // }
            DatabaseChangedEventHandler?.Invoke(this,
                new DatabaseChangedEventArgs() { Operation = CrudOperation.Delete });
            return true;
        }
        catch (Exception e)
        {
            e.LogException(_exceptions);
            return false;
        }
    }

    /// <summary>
    ///     Returns a Dictionary, where the Key is the
    ///     <see cref="Autodesk.Revit.DB.ElementId" />
    ///     used to store the element, and the <see cref="RevitElement" /> as value.
    /// </summary>
    /// <param name="id">An enumerable containing all the ID's to query.</param>
    /// <param name="doc">Revit Document</param>
    /// <returns>True if the operation was completed successfully, false otherwise.</returns>
    public IEnumerable<RevitElement?> Read(string[] elementUniqueIds, Document doc)
    {
        Debug.Print($"\tReading Enumerable from Document: {doc.GetDocumentUuidHash()}::{doc.Title}");
        return GetDocumentElements(doc).Where(x => elementUniqueIds.Contains(x!.Value.ElementUniqueId)); 

        // if (docElements is not null)
        //     foreach (var revitElement in docElements)
        //     {
        //         if (revitElement.ElementId is null) continue;
        //         foreach (var elementId in elementIds)
        //         {
        //             if (!elementId.Value.Equals
        //                     (revitElement.ElementId.Value))
        //                 continue;
        //             TransactionCache.Add
        //             (Transaction.CreateAsCacheItem
        //                 (elementId,
        //                     doc,
        //                     CrudOperation.Read,
        //                     DataType.Element),
        //                 Policy);
        //
        //             yield return revitElement;
        //         }
        //     }

        // DatabaseChangedEventHandler?.Invoke
        // (this,
        //     new DatabaseChangedEventArgs { Operation = CrudOperation.Read });
    }

    /// <summary>
    ///     Returns a RevitElement matching the
    ///     <see cref="Autodesk.Revit.DB.ElementId" /> in the args.
    /// </summary>
    /// <param name="id">The ID used as a key to store the element inside the cache.</param>
    /// <param name="doc">Revit Document</param>
    /// <returns>The requested element. Null if it's not found.</returns>
    public RevitElement? Read(string id, Document doc)
    {
        DatabaseChangedEventHandler?.Invoke(this, new DatabaseChangedEventArgs() { Operation = CrudOperation.Read });
        Debug.Print($"\tReading Element from Document: {doc.GetDocumentUuidHash()}::{doc.Title}");
        return GetDocumentElements(doc).Where(x => x is not null).Select(x => x!.Value).FirstOrDefault();
    }

    public static void HookTimers(ControlledApplication controlledApplication)
    {
        _controlledApplication = controlledApplication;
        _controlledApplication.DocumentOpening += (sender, args) =>
        {
            _ = args;
            Debug.Print($"Document is opening: at {DateTime.UtcNow}");
        };
        _controlledApplication.DocumentClosing += (sender, args) =>
        {
            _ = args;
            Debug.Print(
                $"Document is closing: {args.Document.CreationGUID}::{args.Document.Title}at: {DateTime.UtcNow}");
        };
    }

    private static IEnumerable<RevitElement?> GetDocumentElements(Document doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        Debug.Print($"\tGetting DB from Document: {doc.GetDocumentUuidHash()}::{doc.Title}");
        // foreach (var cachedElement in Wolfden)
        // {
        //     var dict = new Dictionary<string, List<RevitElement>>();
        //     if (cachedElement.Value is not RevitElement r) continue;
        //     if (!dict.TryGetValue(r.ElementUniqueId, out var list))
        //     {
        //         list = [];
        //         dict[r.ElementUniqueId] = list;
        //     }
        //
        //     dict[r.ElementUniqueId].Add(r);
        //     foreach (var found in dict)
        //     {
        //         yield return (found.Value.OrderByDescending(x => x.Id.TimestampMilliseconds).First());
        //     }
        // }
        return ElementCache
            .Where(cachedElement => cachedElement.Value is RevitElement)
            .Select(cachedElement => (RevitElement?)cachedElement.Value)
            .Where(x => x.HasValue)
            .GroupBy(r => r!.Value.ElementUniqueId)
            .Select(found => found.OrderByDescending(x => x!.Value.Id.TimestampMilliseconds).First())
            .AsEnumerable();
        // try
        // {
        //     // return Wolfden.Where(x => x.Key.Contains(doc.GetDocumentTrackingId()))
        //     //     .Where(x => x.Value is RevitElement).Select(x => (RevitElement)x.Value).GroupBy(y => y.ElementUniqueId)
        //     //     .Select(y => y.OrderByDescending(z => z.Id.TimestampMilliseconds).First()).ToArray();
        // }
        // catch (Exception e)
        // {
        //     e.LogException(_exceptions);
        //     return null;
        // }
    }
}