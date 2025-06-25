using System.Collections;
using System.Runtime.Caching;
using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.LLM;
using Direwolf.Definitions.PlatformSpecific;
using Direwolf.Definitions.PlatformSpecific.Extensions;
using Exception = System.Exception;

// ReSharper disable HeapView.ObjectAllocation.Evident
// ReSharper disable HeapView.ClosureAllocation
// ReSharper disable HeapView.DelegateAllocation
// ReSharper disable HeapView.ObjectAllocation.Possible
// ReSharper disable HeapView.ObjectAllocation

/*
 * To do: 2025-06-23
 *
 * Changing dicts to anonymous objs
 *      - prolly better for the MCP stuff
 * Views, sheets and schedules
 *      - main focus for testing
 *      - ok cool JSON and shit, now make it useful
 */

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
    private readonly Dictionary<string, string> _loadCache = [];

    internal void LoadCache(Dictionary<string, string> keys)
    {
        foreach (var key in keys)
        {
            _loadCache.Add(key.Key, key.Value);
        }
    }
    public int ElementCacheStorage => _loadCache.Count;
    
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
    public MessageResponse AddOrUpdateRevitElement(string[]? elementUniqueId, Document doc)
    {
        try
        {
            if (elementUniqueId is null || elementUniqueId.Length == 0)
            {
                return MessageResponse.Error;
            }
            
            var elements = elementUniqueId
                .Select(element => RevitElement.Create(doc, element))
                .ToArray();
            
            foreach (var element in elements)
            {
                if (element is null) continue;
                var wp = WolfpackMessage.Create(GlobalDictionary.WolfpackCreate) with
                {
                    Name = element.Value.ElementName ?? string.Empty,
                    Result = element,
                    Id = element.Value.Id.Value!, // cuid
                    Description = $"{element.Value.CategoryType}: {element.Value.BuiltInCategory}",
                    MessageType = MessageResponse.Result.ToString(),
                    Properties = element.Value.Parameters,
                    Type = "object",
                    Uri = $"{GlobalDictionary.RevitElement}/{element.Value.BuiltInCategory}"
                };
                _loadCache.Add(element.Value.ElementUniqueId, element.Value.Id.Value!);
                Wolfden.GetInstance(doc).AddOrUpdate(wp);
            }
            
            // When pushing the Wolfpack to Wolfden, it will already push uniquely-identifiable elements.
            return MessageResponse.Notification;
        }
        catch (Exception e)
        {
            e.LogException(_exceptions);
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
    public MessageResponse DeleteRevitElement(string[]? elementUniqueId, Document doc)
    {
        try
        {
            if (elementUniqueId is null)
            {
                return MessageResponse.Error;
            }
           
            // keys are cached mainly because it's faster to translate to CUID than to iterate to check for matches
            var keysToDelete = _loadCache.Keys.Where(elementUniqueId.Contains).ToArray();
            Wolfden.GetInstance(doc).DirectDelete(keysToDelete);

            // remove frpm key cache if they were already deleted
            foreach (var element in keysToDelete)
            {
                _loadCache.Remove(element);
            }
            return MessageResponse.Notification;
        }
        catch (Exception e)
        {
            e.LogException(_exceptions);
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
    public MessageResponse ReadRevitElements(Cuid[]? elements, Document doc, out IDictionary<string, object>? data)
    {
        try
        {
            if (elements is null)
            {
                data = null;
                return MessageResponse.Error;
            }

            data = Wolfden.GetInstance(doc).DirectRead(elements.Select(x => x.ToString()).ToArray());
            return MessageResponse.Result;
        }
        catch (Exception e)
        {
             e.LogException(_exceptions);
             data = null;
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
    public MessageResponse ReadRevitElements(string[]? elementUniqueId, Document doc, out RevitElement?[] revitElements)
    {
        var found = new List<string>();
        if (elementUniqueId is null)
        {
            revitElements = new RevitElement?[] { };
            return MessageResponse.Error;
        }

        foreach (var key in elementUniqueId)
        {
            if (_loadCache.TryGetValue(key, out var element))
            {
                found.Add(element);
            }
        }
        revitElements = Wolfden.GetInstance(doc)
            .DirectRead(found.ToArray())
            .Where(x => x.Value is RevitElement)
            .Select(x => (RevitElement?)x.Value)
            .ToArray();
        
        return MessageResponse.Result;
    }

    public static MessageResponse GetAllElements(Document doc, out IDictionary<string, object>? elements)
    {
        elements = Wolfden.GetInstance(doc).GetRevitCache();
        return MessageResponse.Result;
    }

    public ObjectCache GetHunterCache() => Wolfden.HunterCache;
}