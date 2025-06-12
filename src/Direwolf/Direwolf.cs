using System.Diagnostics;
using System.Runtime.Caching;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Direwolf.Definitions.Drivers;
using Direwolf.Definitions.Drivers.JSON;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.Internal;
using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.RevitApi;
using Direwolf.EventArgs;
using Exception = System.Exception;
using MemoryCache = System.Runtime.Caching.MemoryCache;

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
    private readonly List<Howl> _exceptions = [];
    private static readonly Dictionary<Document, Wolfden?> WolfdenInstances = [];

    private Direwolf(Document document)
    {
        DatabaseChangedEventHandler += DatabaseEvent;
        _instance = this;
        WolfdenInstances.Add(document, Wolfden.CreateInstance(document));
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
    ///     Adds a new <see cref="RevitElement" /> to the cache, or updates its value.
    /// </summary>
    /// <param name="elementUniqueId">
    ///     The Transaction containing the ElementId to add, and
    ///     any other data to add.
    /// </param>
    /// <param name="doc"></param>
    /// <returns>True if the operation was completed successfully, false otherwise. </returns>
    public Response AddOrUpdateRevitElement(string elementUniqueId, Document doc)
    {
        try
        {
            var wolfden = WolfdenInstances[doc];
            var element = RevitElement.Create(doc, elementUniqueId);
            
            if (wolfden is null || element is null) return Response.Error;

            wolfden.AddOrUpdateElements(
                CreateFromElementUniqueIds(elementUniqueId, doc), out var _);
            Debug.Print($"Adding to Document: {doc.GetDocumentUuidHash()}::{doc.Title}");
            return Response.Result;
        }
        catch (Exception e)
        {
            e.LogException(_exceptions);
            return Response.Error;
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
    /// <param name="elementUniqueId"></param>
    /// <param name="doc">Revit Document</param>
    /// <returns>True if the operation was completed successfully, false otherwise. </returns>
    public Response DeleteRevitElement(string elementUniqueId, Document doc)
    {
        try
        {
            Debug.Print($"\tUpdating from Document: {doc.GetDocumentUuidHash()}::{doc.Title}");
            var wolfden = WolfdenInstances[doc];

            if (wolfden is null) return Response.Error;
            wolfden.RemoveElements(CreateFromElementUniqueIds(elementUniqueId, doc), out var _);
      
            return Response.Result;
        }
        catch (Exception e)
        {
            e.LogException(_exceptions);
            return Response.Error;
        }
    }

    /// <summary>
    ///     Returns a Dictionary, where the Key is the
    ///     <see cref="Autodesk.Revit.DB.ElementId" />
    ///     used to store the element, and the <see cref="RevitElement" /> as value.
    /// </summary>
    /// <param name="id">An enumerable containing all the ID's to query.</param>
    /// <param name="elementUniqueIds"></param>
    /// <param name="doc">Revit Document</param>
    /// <returns>True if the operation was completed successfully, false otherwise.</returns>
    public static Response Read(string[] elementUniqueIds, Document doc, out IEnumerable<RevitElement?> elements)
    {
        Debug.Print($"\tReading Enumerable from Document: {doc.GetDocumentUuidHash()}::{doc.Title}");
        
        elements = (GetDocumentElements(doc) ?? []).Where(x => x.HasValue).Where(x => x is not null && elementUniqueIds.Contains(x.Value.ElementUniqueId)).Select(x => x);
        return Response.Result;
    }

    /// <summary>
    ///     Returns a RevitElement matching the
    ///     <see cref="Autodesk.Revit.DB.ElementId" /> in the args.
    /// </summary>
    /// <param name="id">The ID used as a key to store the element inside the cache.</param>
    /// <param name="doc">Revit Document</param>
    /// <returns>The requested element. Null if it's not found.</returns>
    public Response Read(string id, Document doc, out RevitElement? element)
    {
        DatabaseChangedEventHandler?.Invoke(this, new DatabaseChangedEventArgs() { Operation = Response.Result });
        Debug.Print($"\tReading Element from Document: {doc.GetDocumentUuidHash()}::{doc.Title}");
        
        element = (GetDocumentElements(doc) ?? throw new InvalidOperationException()).FirstOrDefault(x => id.Equals(x?.ElementUniqueId));
        return Response.Result;
    }

    public Response GetWolfden(Document doc, out Wolfden wolfden)
    {
        wolfden = WolfdenInstances[doc] ?? throw new NullReferenceException(nameof(Wolfden));
        return Response.Result;
    }

    //TODO: try load elements from here.
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

    private static Howl CreateFromElementUniqueIds(string elementUniqueId, Document doc)
    {
        var element = RevitElement.Create(doc, elementUniqueId);
        if (element is null) throw new NullReferenceException(nameof(RevitElement));

        return Howl.Create(
            DataType.Object,
            Method.Post,
            new Dictionary<PayloadId, object>()
            {
                [PayloadId.Create(DataType.Object, "object", new Dictionary<string, object>()
                    {
                        [nameof(element.Value.ElementUniqueId)]  = elementUniqueId,
                    })] =
                    element,
            }, RevitElementJsonSchema.RevitElement, $"{nameof(AddOrUpdateRevitElement)}: {elementUniqueId}");
    }

    private static RevitElement?[]? GetDocumentElements(Document doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        Debug.Print($"\tGetting DB from Document: {doc.GetDocumentUuidHash()}::{doc.Title}");
        
        var wolfden = WolfdenInstances[doc];
        var elements = wolfden?.Values;
        return elements?.Where(x => x.HasValue)
            .GroupBy(r => r!.Value.ElementUniqueId)
            .Select(found => found.OrderByDescending(x => x!.Value.Id.TimestampMilliseconds).First())
            .ToArray();
     
    }
}