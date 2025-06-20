using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.LLM;
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
    private readonly List<Wolfpack> _exceptions = [];

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
    public MessageResponse AddOrUpdateRevitElement(string[]? elementUniqueId, Document doc, out Wolfpack? h)
    {
        try
        {
            if (elementUniqueId is null || elementUniqueId.Length == 0)
            {
                h = null;
                return MessageResponse.Error;
            }
            
            // kvp: ElementUniqueId, RevitElement
            var elements = elementUniqueId
                .Select(element => RevitElement.CreateAsCacheItem(doc, element, out _))
                .ToArray();
            
            // Each Revit Element is already sorted by its UniqueId, so a Wolfpack will contain a Dictionary with <ElementUniqueId, ElementItself>
            h = Wolfpack.Add(new Dictionary<string, object> { ["key"] = elements });
            
            // When pushing the Wolfpack to Wolfden, it will already push uniquely-identifiable elements.
            Wolfden.GetInstance(doc).AddOrUpdate(h);
            return MessageResponse.Notification;
        }
        catch (Exception e)
        {
            e.LogException(_exceptions);
            h = null;
            return MessageResponse.Error;
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
    public MessageResponse DeleteRevitElement(string[]? elementUniqueId, Document doc, out Wolfpack? h)
    {
        try
        {
            if (elementUniqueId is null || elementUniqueId.Length == 0)
            {
                h = null;
                return MessageResponse.Error;
            }
            
            var wp = new WolfpackParams(
                "delete",
                string.Empty,
                "object",
                1,
                string.Empty,
                ResultType.Accepted.ToString(),
                "wolfpack://com.autodesk.revit-latest/direwolf/crud/delete",
                new Dictionary<string, object> { ["key"] = elementUniqueId });

            
            // Wolfpack.Delete has an array of element UniqueIds to destroy
            // stores them as string[]?
            h = Wolfpack.Delete(wp);
            
            // Wolfden *knows* 
            Wolfden.GetInstance(doc).Delete(h.Value);
            return MessageResponse.Notification;
        }
        catch (Exception e)
        {
            e.LogException(_exceptions);
            h = null;
            return MessageResponse.Error;
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
    public MessageResponse ReadRevitElements(ElementId[]? element, Document doc, out RevitElement?[]? revitElement)
    {
        try
        {
            var uniqueIds = element?.Select(x => doc.GetElement(x).UniqueId).ToArray();
             var wp = new WolfpackParams(
                            "read",
                            string.Empty,
                            "object",
                            1,
                            string.Empty,
                            ResultType.Accepted.ToString(),
                            "wolfpack://com.autodesk.revit-latest/direwolf/crud/read",
                            new Dictionary<string, object> { ["key"] = uniqueIds! });
             
            // Wolfpack.Read will store all the UniqueId's as [key] = uniqueId?
            var h = Wolfpack.Read(wp);
            
            // <ElementUniqueId, RevitElement>[]
            Wolfden.GetInstance(doc).Read(h, out var e);
            if (e is null)
            {
                revitElement = [];
                return MessageResponse.Error;
            }
            
            // if the key was the Element's UniqueId, then it's guaranteed that it will cast to RevitElement?
            revitElement = e.Values.Where(x => x is RevitElement).Select(x => (RevitElement?)x).ToArray();
            return MessageResponse.Result;
        }
        catch (Exception e)
        {
             e.LogException(_exceptions);
             revitElement = null;
            return MessageResponse.Error;
        }
    }

    /// <summary>
    ///     Returns a RevitElement matching the
    ///     <see cref="Autodesk.Revit.DB.ElementId" /> in the args.
    /// </summary>
    /// <param name="elementUniqueId">The ID used as a key to store the element inside the cache.</param>
    /// <param name="doc">Revit Document</param>
    /// <returns>The requested element. Null if it's not found.</returns>
    public MessageResponse ReadRevitElements(string[]? uniqueIds, Document doc, out RevitElement?[] revitElements)
    {
         var wp = new WolfpackParams(
                                    "read",
                                    string.Empty,
                                    "object",
                                    1,
                                    string.Empty,
                                    ResultType.Accepted.ToString(),
                                    "wolfpack://com.autodesk.revit-latest/direwolf/crud/read_array",
                                    new Dictionary<string, object> { ["key"] = uniqueIds! });
        // Wolfpack.Read will store all the UniqueId's as [key] = uniqueIds
        var h = Wolfpack.Read(wp);
            
        // <ElementUniqueId, RevitElement>[]
        Wolfden.GetInstance(doc).Read(h, out var e);
        if (e is null)
        {
            revitElements = [];
            return MessageResponse.Error;
        }

        // if the key was the Element's UniqueId, then it's guaranteed that it will cast to RevitElement?
        revitElements = e.Values.Where(x => x is RevitElement).Select(x => (RevitElement?)x).ToArray();
        return MessageResponse.Result;
       
    }

    public static MessageResponse GetAllElements(Document doc, out IDictionary<string, object>? elements)
    {
        elements = Wolfden.GetInstance(doc).GetCache();
        return MessageResponse.Result;
    }
}