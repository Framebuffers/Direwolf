using System.Runtime.Caching;
using Direwolf.Definitions;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.LLM;

namespace Direwolf;

//TODO: refactor to use Wolfpack for everything **except** finalised data out.
public sealed class Hunter : IDirewolfClient
{
    private static readonly object Lock = new();
    private static Hunter? _instance;
    private static Direwolf? _direwolf;
    private static readonly ObjectCache DataCache = MemoryCache.Default; // private cache
    private static ObjectCache? Wolfden => _direwolf?.GetHunterCache();
    private static readonly CacheItemPolicy Policy = new() { SlidingExpiration = TimeSpan.FromMinutes(60) };
    public const string McpProtocolVersion = "2025-06-18";

    private Hunter(Direwolf? direwolf)
    {
        _direwolf = direwolf;
    }

    public static Hunter GetInstance(Direwolf direwolf)
    {
        if (_instance is not null) return _instance;
        lock (Lock)
        {
            if (_instance is not null) return _instance;
            _instance = new Hunter(direwolf);
            return _instance;
        }
    }

    public static async Task<bool> Import()
    {
        if (_direwolf is null) throw new NullReferenceException();
        foreach (var element in Wolfden!)
        {
            DataCache.Add(new CacheItem(element.Key, element.Value), Policy);
        }
        return await Task.FromResult(true);
    }
    
    public static async Task<bool> Export()
    {
        if (Wolfden is null) throw new NullReferenceException();
        foreach (var element in DataCache)
        {
            Wolfden.Add(new CacheItem(element.Key, element.Value), Policy);
        }
        return await Task.FromResult(true);
    }
    
    /// <summary>
    /// When a Wolfpack is created, it will strip the <see cref="WolfpackMessage.Parameters"/> dictionary's data
    /// and do two things: create a CacheElement, delete the entry from the dictionary.
    /// Whenever a Wolfpack is returned with an empty Payload, means it loaded all elements to Cache. 
    /// </summary>
    /// <param name="wolfpackMessage"></param>
    /// <returns></returns>
    public Task<Wolfpack> CreateAsync(in WolfpackMessage wolfpackMessage)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(wolfpackMessage.Parameters);
            Crud(in wolfpackMessage, Operation.Create, out var wolfpack);
            return Task.FromResult(wolfpack with { RequestType = RequestType.Post, MessageResponse = MessageResponse.Notification});
        }
        catch (Exception e)
        {
            return (Task<Wolfpack>)Task.FromException(e);
        }
    }

    /// <summary>
    /// Takes the keys inside <see cref="WolfpackMessage.Parameters"/> and looks for them inside the <see cref="DataCache"/>.
    /// If found, it will remove the element with the same key, and add another with the values stored in Parameters.
    /// <remarks>
    /// All operations are one-on-one with <see cref="WolfpackMessage.Parameters"/>: all key-value pair inside the <see cref="WolfpackMessage"/>
    /// equals a key-value pair inside <see cref="DataCache"/>. Use this property to manipulate the database directly.
    /// </remarks>
    /// </summary>
    /// <param name="wolfpackMessage"></param>
    /// <returns></returns>
    public Task<Wolfpack> UpdateAsync(in WolfpackMessage wolfpackMessage)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(wolfpackMessage);
            ArgumentNullException.ThrowIfNull(wolfpackMessage.Parameters);
            Crud(in wolfpackMessage, Operation.Update, out var wolfpack);
            return Task.FromResult(wolfpack with { RequestType = RequestType.Patch, MessageResponse = MessageResponse.Notification});
        }
        catch (Exception e)
        {
            return (Task<Wolfpack>)Task.FromException(e);
        }
    }

    /// <summary>
    /// Looks for all cached elements with the same keys as the ones inside <see cref="WolfpackMessage.Parameters"/> and
    /// deletes them from the <see cref="DataCache"/>.
    /// </summary>
    /// <param name="wolfpackMessage"></param>
    /// <returns></returns>
    public Task<Wolfpack> DeleteAsync(in WolfpackMessage wolfpackMessage)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(wolfpackMessage.Parameters);
            Crud(in wolfpackMessage, Operation.Delete, out var wolfpack);
            return Task.FromResult(wolfpack with { RequestType = RequestType.Delete, MessageResponse = MessageResponse.Notification});
        }
        catch (Exception e)
        {
            return (Task<Wolfpack>)Task.FromException(e);
        }
    }

    /// <summary>
    /// Looks for all the cached elements matching all keys inside <see cref="WolfpackMessage.Parameters"/>, and
    /// returns a Wolfpack with the values filled.
    ///
    /// <remarks>
    /// To read values, add the keys to be read to the dictionary and leave their value null. If successful, all found
    /// values will be paired to their requested key.
    /// </remarks>
    /// </summary>
    /// <param name="wolfpackMessage"></param>
    /// <returns></returns>
    public Task<Wolfpack> GetAsync(in WolfpackMessage wolfpackMessage)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(wolfpackMessage.Parameters);
            var decode = (IDictionary<string, object>)wolfpackMessage.Parameters["keys"];
            if (decode.Count == 0)
            {
                var wp = Wolfpack.Create(wolfpackMessage.Name, MessageResponse.Error, RequestType.Patch,
                    wolfpackMessage.Parameters, wolfpackMessage.Description);
                wp = wp with
                {
                    McpResponseResult = null, Parameters = null
                };
                return Task.FromResult(wp);
            }

            var addedPayload = wolfpackMessage with { Parameters = decode };
            Crud(in addedPayload, Operation.Read, out var wolfpack);
            return Task.FromResult(wolfpack with { RequestType = RequestType.Get, Parameters = wolfpack.Parameters, MessageResponse = MessageResponse.Result});
        }
        catch (Exception e)
        {
            return (Task<Wolfpack>)Task.FromException(e);
        }
    }

    /// <inheritdoc cref="GetAsync"/>>
    /// <inheritdoc/>>
    /// <param name="wolfpackMessage"></param>
    /// <returns></returns>
    public Task<Wolfpack> GetManyAsync(in WolfpackMessage wolfpackMessage)
    {
        try
        {
            Crud(in wolfpackMessage, Operation.ReadArray, out var wolfpack);
            return Task.FromResult(wolfpack with { RequestType = RequestType.Get, MessageResponse = MessageResponse.Result});
        }
        catch (Exception e)
        {
            return (Task<Wolfpack>)Task.FromException(e);
        }
    }


    private enum Operation
    {
        Create,
        Read,
        ReadArray,
        Update,
        Delete
    }

    private void Crud(in WolfpackMessage input, Operation op, out Wolfpack output)
    {
        var wp = Wolfpack.Create(input.Name, MessageResponse.Notification, RequestType.Patch, input.Parameters,
            input.Description);
        if (input.Parameters is null) throw new NullReferenceException();
        var incomingDictionary = input.Parameters;
        if (incomingDictionary is null)
            output = wp with
            {
                MessageResponse = MessageResponse.Error, McpResponseResult = null, Parameters = null
            };
        var counter = 0;
        foreach (var incomingKey in incomingDictionary!)
        {
            var cacheKey = new CacheItem(incomingKey.Key, incomingDictionary[incomingKey.Key]);
            switch (op)
            {
                case Operation.Create:
                    DataCache.Add(new CacheItem(incomingKey.Key, incomingDictionary[incomingKey.Key]), Policy);
                    counter++;
                    break;
                case Operation.Read:
                    var readValue = DataCache.Get(cacheKey.Key);
                    wp = wp with { McpResponseResult = readValue, Parameters = null};
                    counter++;
                    break;
                case Operation.ReadArray:
                    var values = DataCache.GetValues(incomingDictionary.Keys);
                    wp = wp with { McpResponseResult = values };
                    counter = +incomingDictionary.Count;
                    break;
                case Operation.Update:
                    if (!DataCache.Contains(incomingKey.Key)) continue;
                    DataCache.Remove(incomingKey.Key);
                    DataCache.Add(cacheKey, Policy);
                    counter++;
                    break;
                case Operation.Delete:
                    if (!DataCache.Contains(incomingKey.Key)) continue;
                    DataCache.Remove(incomingKey.Key);
                    counter++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(op), op, null);
            }
        }

        output = wp with
        {
            MessageResponse = MessageResponse.Result,
            Description = new
            {
                incoming = incomingDictionary.Count,
                updated = counter,
                difference = counter,
                checkPassed = counter != 0
            }.ToString()
        };
}

    private static McpResponse NotificationAccepted =>
        new(null, $"message: {MessageResponse.Notification}, result: {ResultType.Accepted}", null);

    private static McpResponse NotificationRejected =>
        new(null, $"message: {MessageResponse.Notification}, result: {ResultType.Rejected}", null);

    private static McpResponse ResultAccepted =>
        new(null, $"message: {MessageResponse.Notification}, result: {ResultType.Accepted}", null);

    private static McpResponse ResultRejected =>
        new(null, $"message: {MessageResponse.Notification}, result: {ResultType.Rejected}", null);

    private static McpResponse RequestAccepted =>
        new(null, $"message: {MessageResponse.Request}, result: {ResultType.Accepted}", null);
}