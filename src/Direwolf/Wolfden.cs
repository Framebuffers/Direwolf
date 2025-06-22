using System.Runtime.Caching;
using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.PlatformSpecific.Extensions;

namespace Direwolf;

internal sealed class Wolfden
{
    private static Wolfden? _instance;
    private static readonly object Lock = new();
    private readonly ObjectCache _revitCache = MemoryCache.Default;
    public static ObjectCache HunterCache = MemoryCache.Default;
    private static readonly CacheItemPolicy Policy = new() { SlidingExpiration = TimeSpan.FromMinutes(60) };
    private readonly Queue<Wolfpack?> _operationQueue = [];
    private readonly Stack<Wolfpack?> _transactionStack = [];
    internal readonly Dictionary<string, Cuid> RevitElementKeyCache = [];
    private readonly Document? _doc;

    private Wolfden(Document doc)
    {
        _instance = this;
        
        PopulateDatabase(doc);
    }
    
    public IDictionary<string, object> GetRevitCache() => _revitCache.ToDictionary();

    public static Wolfden GetInstance(Document? doc)
    {
        if (_instance is not null) return _instance;
        lock (Lock)
        {
            if (_instance is not null) return _instance;
            _instance = new Wolfden(doc!);
            return _instance;
        }
    }

    public MessageResponse PopulateDatabase(Document doc)
    {
        try
        {
            // These cache items have the Revit Element's UniqueId as a key, and the RevitElement object as values.
            var revitDb = doc.GetRevitDatabaseAsCacheItems();
            foreach (var cacheItem in revitDb)
            {
                if (cacheItem is null) continue;
                _revitCache.Add(cacheItem, Policy);
            }

            return MessageResponse.Result;
        }
        catch
        {
            return MessageResponse.Error;
        }
    }

    public MessageResponse AddOrUpdate(Wolfpack? entity)
    {
        try
        {
            // Inside Direwolf, the AddOrUpdateRevitElement() method will take
            // each Element's UniqueId, create a RevitElement as a CacheItem, and put them on an array.
            // We know what type it's going to be, so we can cast it safely.
            if (entity?.Parameters is null) return MessageResponse.Error;
            var items = (CacheItem?[])entity.Value.Parameters["key"];

            foreach (var item in items)
            {
                if (item is null) continue;
                _revitCache.Add(item, Policy);
            }

            entity = entity.Value with { McpResponseResult = ResultType.Accepted };
            _transactionStack.Push(entity);

            return MessageResponse.Notification;
        }
        catch
        {
            return MessageResponse.Error;
        }
    }
    

    public MessageResponse Delete(Wolfpack? entity)
    {
        // We know that the Wolfpack's payload value is string[]?, if it was constructed using Direwolf
        // Given Wolfden is Internal, all operations should go through Direwolf.
        // So this shouldn't be a problem... right?
        var keyToRemove = (string[]?)entity?.Parameters?["key"];

        if (keyToRemove is null) return MessageResponse.Error;
        foreach (var item in keyToRemove)
        {
            _revitCache.Remove(item);
        }
        
        entity = entity!.Value with {  McpResponseResult = ResultType.Accepted, RequestType = RequestType.Delete};
        _transactionStack.Push(entity);
     
        return MessageResponse.Notification;
    }

    public MessageResponse Read(Wolfpack? entity, out IDictionary<string, object>? results)
    {
        if (entity?.Parameters is null)
        {
            results = null;
            return MessageResponse.Error;
        }
        
        // Same as adding, we know the type of type being loaded onto that object.
        // This time, Direwolf is asking for string[]?
        var items = (string[]?)entity.Value.Parameters!["key"];
        
        // Because all elements were stored as <ElementUniqueId, RevitElement>, it can be safely cast back to RevitElement
        var found = _revitCache.GetValues(items!);
        results = found;
        return MessageResponse.Notification;
    }
    


    public bool PopTransaction(out Wolfpack? element)
    {
        return _transactionStack.TryPop(out element);
    }
}