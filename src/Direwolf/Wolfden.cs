using System.Runtime.Caching;
using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.PlatformSpecific.Extensions;
using Direwolf.Definitions.Serialization;

namespace Direwolf;

internal sealed class Wolfden
{
    private static Wolfden? _instance;
    private static readonly object Lock = new();
    private readonly ObjectCache _elementCache = MemoryCache.Default;
    private static readonly CacheItemPolicy Policy = new() { SlidingExpiration = TimeSpan.FromMinutes(60) };
    private readonly Queue<Howl?> _operationQueue = [];
    private readonly Stack<Howl?> _transactionStack = [];
    internal readonly Dictionary<string, Cuid> RevitElementKeyCache = [];
    private readonly Document? _document;

    private Wolfden(Document document)
    {
        _instance = this;
        _document = document;
        if (PopulateDatabase() is MessageType.Error) throw new InvalidOperationException(nameof(Wolfden));
    }
    
    
    public IDictionary<string, object> GetCache() => _elementCache.ToDictionary();

    public static Wolfden GetInstance(Document doc)
    {
        if (_instance is not null) return _instance;
        lock (Lock)
        {
            if (_instance is not null) return _instance;
            _instance = new Wolfden(doc);
            return _instance;
        }
    }

    private MessageType PopulateDatabase()
    {
        try
        {
            // These cache items have the Revit Element's UniqueId as a key, and the RevitElement object as values.
            var revitDb = _document?.GetRevitDatabaseAsCacheItems();
            if (revitDb is null) return MessageType.Error;
            foreach (var cacheItem in revitDb)
            {
                if (cacheItem is null) continue;
                _elementCache.Add(cacheItem, Policy);
            }

            return MessageType.Result;
        }
        catch
        {
            return MessageType.Error;
        }
    }

    public MessageType AddOrUpdate(Howl? howl)
    {
        try
        {
            // Inside Direwolf, the AddOrUpdateRevitElement() method will take
            // each Element's UniqueId, create a RevitElement as a CacheItem, and put them on an array.
            // We know what type it's going to be, so we can cast it safely.
            if (howl?.Properties is null) return MessageType.Error;
            var items = (CacheItem?[])howl.Value.Properties["key"];

            foreach (var item in items)
            {
                if (item is null) continue;
                _elementCache.Add(item, Policy);
            }

            howl = howl.Value with { Result = ResultType.Accepted };
            _transactionStack.Push(howl);

            return MessageType.Notification;
        }
        catch
        {
            return MessageType.Error;
        }
    }

    public MessageType Delete(Howl? howl)
    {
        // We know that the Howl's payload value is string[]?, if it was constructed using Direwolf
        // Given Wolfden is Internal, all operations should go through Direwolf.
        // So this shouldn't be a problem... right?
        var keyToRemove = (string[]?)howl?.Properties?["key"];

        if (keyToRemove is null) return MessageType.Error;
        foreach (var item in keyToRemove)
        {
            _elementCache.Remove(item);
        }
        
        howl = howl!.Value with { Result = ResultType.Accepted };
        _transactionStack.Push(howl);
     
        return MessageType.Notification;
    }

    public MessageType Read(Howl? howl, out IDictionary<string, object?>? results)
    {
        if (howl?.Properties is null)
        {
            results = null;
            return MessageType.Error;
        }
        
        // Same as adding, we know the type of type being loaded onto that object.
        // This time, Direwolf is asking for string[]?
        var items = (string[]?)howl.Value.Properties["key"];
        
        // Because all elements were stored as <ElementUniqueId, RevitElement>, it can be safely cast back to RevitElement
        var found = _elementCache.GetValues(items!);
        results = found;
        return MessageType.Notification;
    }

    public bool PopTransaction(out Howl? element)
    {
        return _transactionStack.TryPop(out element);
    }
}