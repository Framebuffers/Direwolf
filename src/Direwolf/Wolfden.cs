using System.Runtime.Caching;
using System.Text.Json;
using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.LLM;
using Direwolf.Definitions.PlatformSpecific;
using Direwolf.Definitions.PlatformSpecific.Extensions;
using YamlDotNet.Core.Tokens;

namespace Direwolf;

public sealed class Wolfden
{
    private static Wolfden? _instance;
    private static readonly object Lock = new();
    private readonly ObjectCache _revitCache = MemoryCache.Default;
    public static ObjectCache HunterCache = MemoryCache.Default;
    private static readonly CacheItemPolicy Policy = new() { SlidingExpiration = TimeSpan.FromMinutes(60) };

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
            var revitDb = doc.GetRevitDatabaseAsCacheItems(out var kvpCache);
            
            
            foreach (var cacheItem in revitDb)
            {
                if (cacheItem is null) continue;
                _revitCache.Add(cacheItem, Policy);
            }
            Direwolf.GetInstance().LoadCache(kvpCache);
            return MessageResponse.Result;
        }
        catch
        {
            return MessageResponse.Error;
        }
    }
    
    public MessageResponse AddOrUpdate(WolfpackMessage? entity)
    {
        try
        {
            // Inside Direwolf, the AddOrUpdateRevitElement() method will take
            // each Element's UniqueId, create a RevitElement as a CacheItem, and put them on an array.
            // We know what type it's going to be, so we can cast it safely.
            if (entity?.Properties is null) return MessageResponse.Error;
            var encodedWolfpack = Encode(entity);
            _revitCache.Add(encodedWolfpack, Policy); 

            return MessageResponse.Notification;
        }
        catch
        {
            return MessageResponse.Error;
        }
    }
    

    public MessageResponse Delete(WolfpackMessage? entity)
    {
        // We know that the Wolfpack's payload value is string[]?, if it was constructed using Direwolf
        // Given Wolfden is Internal, all operations should go through Direwolf.
        // So this shouldn't be a problem... right?
        if (entity?.Properties is null) return MessageResponse.Error;
        var encodedWolfpack = Encode(entity); 
        _revitCache.Remove(encodedWolfpack.Key); 
     
        return MessageResponse.Notification;
    }

    internal IDictionary<string, object> DirectRead(string[] keys) => _revitCache.GetValues(keys);
    internal void DirectWrite(CacheItem[] objects)
    {
        foreach (var o in objects)
        {
            _revitCache.Add(o, Policy);
        }
    }

    internal void DirectDelete(string[] keys)
    {
        foreach (var o in keys)
            _revitCache.Remove(o);
    }

    public MessageResponse Read(WolfpackMessage? entity, out IDictionary<string, object>? results)
    {
        if (entity?.Result is null)
        {
            results = null;
            return MessageResponse.Error;
        }
        
        results = _revitCache.GetValues(entity.Value.Id);
        return MessageResponse.Notification;
    }

    public MessageResponse Read(Document doc, Cuid ids, ObjectCache cache, out RevitElement? results)
    {
        if (cache.Contains(ids.Value!))
            results = (RevitElement)cache[ids.Value!];
        else
            results = null;
        return MessageResponse.Notification;
    }

    /// <summary>
    /// Extracts the <see cref="WolfpackMessage.Id"/> and <see cref="WolfpackMessage.Result"/> as a native CacheItem for
    /// the internal <see cref="ObjectCache"/> 
    /// </summary>
    /// <param name="element">Message containing data to store.</param>
    /// <returns>Native CacheItem to be used with ObjectCache</returns>
    private static CacheItem Encode(WolfpackMessage? element)
    {
        return new CacheItem(element!.Value.Id, element.Value);
    }

    private static IDictionary<string, object> Decode(IEnumerable<Cuid> key, ObjectCache cache)
    {
        return cache.GetValues(key.Select(x => x.Value));
    }
    private static IDictionary<string, object> Decode(string[] keys, ObjectCache cache)
    {
        return cache.GetValues(keys);
    }
    
}