using System.Collections.Concurrent;
using System.Runtime.Caching;
using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.Parsers;
using Direwolf.Definitions.RevitApi;

namespace Direwolf;

public sealed class Wolfden
{
    private static Wolfden? _instance;
    private static readonly object Lock = new();
    private static readonly ObjectCache ElementCache = MemoryCache.Default;
    private static readonly CacheItemPolicy Policy = new() { SlidingExpiration = TimeSpan.FromMinutes(60) };
    private readonly Queue<Howl?> _operationQueue = [];
    private readonly Stack<Howl?> _transactionStack = [];
    internal readonly Dictionary<string, Cuid> KeyCache = [];
    private static Document? _document;
    
    private Wolfden(Document document)
    {
        _instance = this;
        _document = document;
        if (PopulateDatabase() is MessageType.Error) throw new InvalidOperationException(nameof(Wolfden));
    }
    
    public static IDictionary<string, object> GetCache() => ElementCache.ToDictionary();
    
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

    private static MessageType PopulateDatabase()
    {
        try
        {
            var revitDb = _document?.GetRevitDatabaseAsCacheItems();
            if (revitDb is null) return MessageType.Error;
            foreach (var cacheItem in revitDb)
            {
                if (cacheItem is null) continue;
                ElementCache.Add(cacheItem, Policy);
            }

            return MessageType.Result;
        }
        catch
        {
            return MessageType.Error;
        }
    }

    public MessageType AddOrUpdateElements(Howl? r)
    {
        try
        {
            if (r is null)
            {
                return MessageType.Error;
            }

            var element = Howl.AsCacheItem(r);
            if (ElementCache.Contains(element.Key)) ElementCache.Remove(element.Key);
            ElementCache.Add(Howl.AsCacheItem(r), Policy);
            _transactionStack.Push(r);
            // foreach (var element in results)
            // {
            //     if (element is null) continue;
            //     // AddOrUpdate(Guid.Parse(element.Value.ElementUniqueId), guid =>
            //     // {
            //     //     // ReSharper disable once ConvertToLambdaExpression
            //     //     return RevitElement.Create(document, guid.ToString());
            //     // }, (guid, revitElement) =>
            //     // {
            //     //     _transactionStack.Push(this[guid]); // save the old value
            //     //     return this[guid] = revitElement;
            //     // });
            // }

            return MessageType.Result;
        }
        catch
        {
            return MessageType.Error;
        }
    }

    public MessageType RemoveElements(Howl h)
    {
        if (h.Payload is null)
        {
            return MessageType.Error;
        }
        var keyToRemove = (string)h.Payload["key"];
        
        ElementCache.Remove(keyToRemove);
        _transactionStack.Push(h);
        // var results = Extract(h) ?? throw new NullReferenceException(nameof(RevitElement));
        // List<RevitElement?> output = [];
        // foreach (var result in results)
        // {
        //     if (result?.ElementUniqueId is null) continue;
        //     TryRemove(Guid.Parse(result.Value.ElementUniqueId), out var extracted);
        //     output.Add(extracted);
        // }
        //
        // element = output.ToArray();
        return MessageType.Result;
    }

    public static MessageType ReadElements(Howl h, out IDictionary<string, object?>? results)
    {
        if (h.Payload is null)
        {
            results = null;
            return MessageType.Error;
        }

        var values = (Cuid[])h.Payload["key"];
        var found = ElementCache.GetValues(values.Select(x => x.Value));
        results = found;
        return MessageType.Result;
    }

    public bool PopTransaction(out Howl? element)
    {
        return _transactionStack.TryPop(out element);
    }

}