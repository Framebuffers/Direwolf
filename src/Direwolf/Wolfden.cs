using System.Collections.Specialized;
using System.Runtime.Caching;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Direwolf.Definitions;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.RevitApi;
using Direwolf.EventArgs;
using Microsoft.Extensions.Caching.Memory;

namespace Direwolf;

public class Wolfden
{
    private static readonly object Lock = new();
    private static readonly Dictionary<Document, ElementCache?> LoadedDocuments = [];
    private static Wolfden? _instance;

    private Wolfden(Document doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        LoadedDocuments.TryAdd(doc, new ElementCache(doc.CreationGUID.ToString(), doc));
    }

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

    public static ElementCache? GetOrCreateElementCache(Document doc)
    {
        LoadedDocuments.TryGetValue(doc, out var elementCache);
        if (elementCache is not null) return elementCache;
        LoadedDocuments.TryAdd(doc, new ElementCache(doc.CreationGUID.ToString(), doc));
        return elementCache;
    }

    public static bool DeleteElementCache(Document doc)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(doc);
            LoadedDocuments.Remove(doc);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public class ElementCache : MemoryCache
    {
        private static readonly List<RevitElement?> Transactions = [];

        private static readonly CacheItemPolicy InternalElementPolicy = new()
        {
            SlidingExpiration = TimeSpan.FromMinutes(60)
        };

        public ElementCache(string name, Document doc, NameValueCollection config = null!) : base(name, config)
        {
            ArgumentNullException.ThrowIfNull(doc);
            if (!PopulateDatabase(doc, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(10) }))
                throw new InvalidOperationException("Failed to populate database");
        }

        public ElementCache(string name, Document doc, NameValueCollection config, bool ignoreConfigSection) : base(
            name, config, ignoreConfigSection)
        {
            ArgumentNullException.ThrowIfNull(doc);
            if (!PopulateDatabase(doc, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(10) }))
                throw new InvalidOperationException("Failed to populate database");
        }

        public bool RecordTransaction(RevitElement edit, Document doc)
        {
            ArgumentNullException.ThrowIfNull(doc);
            try
            {
                var existingItem = (RevitElement?)Get(edit.ElementUniqueId);
                if (existingItem is not null) Transactions.Add(edit);
                var newItem = RevitElement.CreateAsCacheItem(doc, edit.ElementUniqueId, out _);
                var x = existingItem.Value with
                {
                    Parameters = edit.Parameters
                };
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool PopulateDatabase(Document doc, CacheItemPolicy? policy)
        {
            try
            {
                foreach (var cacheItem in doc.GetRevitDatabaseAsCacheItems())
                {
                    if (cacheItem is null) continue;
                    Add(cacheItem, policy ?? new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromMinutes(60) });
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static T? GetOrAddCacheItem<T>(CacheItem? cacheItem, ElementCache cacheToCheck, string keyForNewItem, Func<T> funcToCreateValue)
        {
            var existingItem = cacheToCheck.Get(cacheItem.Key);
            if (existingItem is not null) return (T?)existingItem;
            var newItem = funcToCreateValue();
            if (newItem is null) return default;
            cacheToCheck.Add(keyForNewItem, newItem, InternalElementPolicy);

            return newItem;
        }
    }
}