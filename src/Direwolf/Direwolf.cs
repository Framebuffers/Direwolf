using System.Diagnostics;
using System.Runtime.Caching;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.Parsers;
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

    // ReSharper disable once MemberCanBePrivate.Global
    private static ControlledApplication? _controlledApplication;
    private static Direwolf? _instance;
    private readonly List<Howl> _exceptions = [];
    private static Wolfden? _wolfden;

    private Direwolf(Document document)
    {
        DatabaseChangedEventHandler += DatabaseEvent;
        _instance = this;
        _wolfden = Wolfden.GetInstance(document);
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
    public MessageType AddOrUpdateRevitElement(string elementUniqueId, Document doc)
    {
        try
        {
            var element = RevitElement.Create(doc, elementUniqueId);
            if (element is null) return MessageType.Error;
            var howl = Howl.Create(DataType.Object, RequestType.Put,
                new Dictionary<string, object>() { [elementUniqueId] = element });
            var cache = Wolfden.GetInstance(doc).KeyCache;
            if (!cache.TryGetValue(elementUniqueId, out var cachedElement))
            {
                cachedElement = howl.Id;
                cache.Add(elementUniqueId, cachedElement);
            }
            else
            {
                cache[elementUniqueId] = howl.Id;
            }

            _wolfden?.AddOrUpdateElements(howl);
            Debug.Print($"Adding to Document: {doc.GetDocumentUuidHash()}::{doc.Title}");
            return MessageType.Result;
        }
        catch (Exception e)
        {
            e.LogException(_exceptions);
            return MessageType.Error;
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
    public MessageType DeleteRevitElement(string elementUniqueId, Document doc)
    {
        try
        {
            Debug.Print($"\tUpdating from Document: {doc.GetDocumentUuidHash()}::{doc.Title}");
            var howl = Howl.Create(DataType.String, RequestType.Delete,
                new Dictionary<string, object>() { ["key"] = elementUniqueId, ["value"] = null! });
            var cache = _wolfden?.KeyCache;
            cache?.Remove(elementUniqueId, out var _);
            _wolfden?.RemoveElements(howl);
            return MessageType.Result;
        }
        catch (Exception e)
        {
            e.LogException(_exceptions);
            return MessageType.Error;
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
    public static MessageType Read(string[] elementUniqueIds, Document doc, out IEnumerable<RevitElement?>? elements)
    {
        Debug.Print($"\tReading Enumerable from Document: {doc.GetDocumentUuidHash()}::{doc.Title}");
        var l = new List<RevitElement?>();
        var cuids = new List<Cuid>();
        var cache = _wolfden?.KeyCache;
        foreach (var id in elementUniqueIds)
        {
            if (cache is null || !cache.TryGetValue(id, out var cachedElement)) continue;
            cuids.Add(cachedElement);
        }

        var howl = Howl.Create(DataType.Array, RequestType.Get,
            new Dictionary<string, object>() { ["key"] = cuids.ToArray() });
        Wolfden.ReadElements(howl, out var e);
      
        elements = e.Values.Where(x => x is RevitElement).Select(x => (RevitElement?)x).AsEnumerable();
        return MessageType.Result;
    }

    /// <summary>
    ///     Returns a RevitElement matching the
    ///     <see cref="Autodesk.Revit.DB.ElementId" /> in the args.
    /// </summary>
    /// <param name="elementUniqueId">The ID used as a key to store the element inside the cache.</param>
    /// <param name="doc">Revit Document</param>
    /// <returns>The requested element. Null if it's not found.</returns>
    public MessageType Read(string elementUniqueId, Document doc, out RevitElement? element)
    {
        DatabaseChangedEventHandler?.Invoke(this, new DatabaseChangedEventArgs { Operation = MessageType.Result });
        Debug.Print($"\tReading Element from Document: {doc.GetDocumentUuidHash()}::{doc.Title}");
        var howl = Howl.Create(DataType.Object, RequestType.Get,
            new Dictionary<string, object>() { ["key"] = elementUniqueId });
        var cache = Wolfden.GetCache();
        if (!cache.TryGetValue(elementUniqueId, out var cachedElement))
        {
            cachedElement = howl.Id;
            cache.Add(elementUniqueId, cachedElement);
        }

        Wolfden.ReadElements(howl, out var resultElements);
        element = (RevitElement?)resultElements.Values.FirstOrDefault(x => x is RevitElement);
        return MessageType.Result;
    }

    public static MessageType GetWolfden(Document doc, out Wolfden wolfden)
    {
        wolfden = Wolfden.GetInstance(doc);
        return MessageType.Result;
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
        return Howl.Create(DataType.Object, RequestType.Post,
            new Dictionary<string, object>
            {
                [nameof(DataType)] = DataType.Object, [nameof(element.Value.ElementUniqueId)] = elementUniqueId
            }, $"{nameof(AddOrUpdateRevitElement)}: {elementUniqueId}");
    }

    public static Wolfpack CreatePrompt(Document doc, string name, string? description, Howl result, string uri)
    {
        var howl = Howl.Create(DataType.String, RequestType.Get, result.Payload!) with { Result = ResultType.Accepted };
        var args = WolfpackArguments.Create(howl, uri);
        var id = Cuid.CreateRevitId(doc, out var _);
        var results = new[] { Howl.AsPayload(howl) };
        return Wolfpack.Create(id, RequestType.Get, name, args, results, description);
    }
}