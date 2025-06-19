using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.PlatformSpecific;
using Exception = System.Exception;

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
    private static Direwolf? _instance;
    private readonly List<Howl> _exceptions = [];

    private Direwolf()
    {
        _instance = this;
    }

    public static Direwolf GetInstance()
    {
        if (_instance is not null) return _instance;
        lock (Lock)
        {
            if (_instance is not null) return _instance;
            _instance = new Direwolf();
            return _instance;
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
    public MessageType AddOrUpdateRevitElement(string[]? elementUniqueId, Document doc, out Howl? h)
    {
        try
        {
            if (elementUniqueId is null || elementUniqueId.Length == 0)
            {
                h = null;
                return MessageType.Error;
            }

            var elements = elementUniqueId.Select(element => RevitElement.CreateAsCacheItem(doc, element, out var _))
                .ToArray();
            
            // Each Revit Element is already sorted by its UniqueId, so a Howl will contain a Dictionary with <ElementUniqueId, ElementItself>
            h = Howl.Add(new Dictionary<string, object>(){["key"] = elements}, null, null);
            
            // When pushing the Howl to Wolfden, it will already push uniquely-identifiable elements.
            Wolfden.GetInstance(doc).AddOrUpdate(h);
            return MessageType.Notification;
        }
        catch (Exception e)
        {
            e.LogException(_exceptions);
            h = null;
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
    public MessageType DeleteRevitElement(string[]? elementUniqueId, Document doc, out Howl? h)
    {
        try
        {
            if (elementUniqueId is null || elementUniqueId.Length == 0)
            {
                h = null;
                return MessageType.Error;
            }

            
            // Howl.Delete has an array of element UniqueIds to destroy
            // stores them as string[]?
            h = Howl.Delete(elementUniqueId, null, null);
            // Wolfden *knows* 
            Wolfden.GetInstance(doc).Delete(h.Value);
            return MessageType.Notification;
        }
        catch (Exception e)
        {
            e.LogException(_exceptions);
            h = null;
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
    public MessageType ReadRevitElements(ElementId[]? element, Document doc, out RevitElement?[]? revitElement, out Howl? h)
    {
        try
        {
            var uniqueIds = element?.Select(x => doc.GetElement(x).UniqueId).ToArray();
            
            // Howl.Read will store all the UniqueId's as [key] = uniqueId?
            h = Howl.Read(uniqueIds);
            
            // <ElementUniqueId, RevitElement>[]
            Wolfden.GetInstance(doc).Read(h.Value, out var e);
            if (e is null)
            {
                revitElement = [];
                return MessageType.Error;
            }
            
            // if the key was the Element's UniqueId, then it's guaranteed that it will cast to RevitElement?
            revitElement = e.Values.Where(x => x is RevitElement).Select(x => (RevitElement?)x).ToArray();
            return MessageType.Result;
        }
        catch (Exception e)
        {
             e.LogException(_exceptions);
             revitElement = null;
             h = null;
            return MessageType.Error;
        }
    }

    /// <summary>
    ///     Returns a RevitElement matching the
    ///     <see cref="Autodesk.Revit.DB.ElementId" /> in the args.
    /// </summary>
    /// <param name="elementUniqueId">The ID used as a key to store the element inside the cache.</param>
    /// <param name="doc">Revit Document</param>
    /// <returns>The requested element. Null if it's not found.</returns>
    public MessageType ReadRevitElements(string[]? uniqueIds, Document doc, out RevitElement?[] revitElements, out Howl? h)
    {
        // Howl.Read will store all the UniqueId's as [key] = uniqueIds
        h = Howl.Read(uniqueIds);
            
        // <ElementUniqueId, RevitElement>[]
        Wolfden.GetInstance(doc).Read(h.Value, out var e);
        if (e is null)
        {
            revitElements = [];
            h = null;
            return MessageType.Error;
        }

        // if the key was the Element's UniqueId, then it's guaranteed that it will cast to RevitElement?
        revitElements = e.Values.Where(x => x is RevitElement).Select(x => (RevitElement?)x).ToArray();
        return MessageType.Result;
       
    }

    public static MessageType GetElementCache(Document doc, out IDictionary<string, object>? elements)
    {
        elements = Wolfden.GetInstance(doc).GetCache();
        return MessageType.Result;
    }
}