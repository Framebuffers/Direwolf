using System.Runtime.Caching;
using Direwolf.Definitions;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.LLM;

namespace Direwolf;

public sealed class Hunter : IDirewolfClient
{
    private static readonly object Lock = new();
    private static Hunter? _instance;
    private static readonly ObjectCache DataCache = MemoryCache.Default;

    private static readonly CacheItemPolicy Policy = new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(60)
    };

    private Hunter()
    {
    }

    public static Hunter GetInstance()
    {
        if (_instance is not null)
            return _instance;
        lock (Lock)
        {
            if (_instance is not null)
                return _instance;
            _instance = new Hunter();
            return _instance;
        }
    }


    public Task<McpResponse> CreateAsync(in WolfpackParams parameters, out Wolfpack? wolfpack)
    {
        try
        {
            var wp = Wolfpack.Create(parameters.Name, MessageResponse.Notification, RequestType.Post, parameters.Properties, parameters.Description);
            
            var cache = Wolfpack.AsCacheItem(wp);
            
            ArgumentNullException.ThrowIfNull(cache);
            foreach (var item in cache)
            {
                DataCache.Add(item, Policy);
            }
            wolfpack = wp;
            return Task.FromResult(NotificationAccepted);
        }
        catch (Exception e)
        {
            wolfpack = null;
            return (Task<McpResponse>)Task.FromException(e);
        }
    }

    public Task<McpResponse> UpdateAsync(in Wolfpack? updateArgs)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(updateArgs);
            if (updateArgs.Value.Id.Value is null)
                throw new NullReferenceException();
            if (!DataCache.Contains(updateArgs.Value.Id.Value)) 
                return Task.FromResult(NotificationRejected);
            DataCache.Remove(updateArgs.Value.Id.Value!);
            var wp = Wolfpack.AsCacheItem(updateArgs)!.FirstOrDefault();
            DataCache.Add(wp!,
                Policy);
            return Task.FromResult(NotificationAccepted);

        }
        catch (Exception e)
        {
            return (Task<McpResponse>)Task.FromException(e);
        }
    }

    public Task<McpResponse> DeleteAsync(in Cuid keys)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(keys.Value);
            DataCache.Remove(keys.Value);
            return Task.FromResult(NotificationAccepted);
        }
        catch (Exception e)
        {
            return (Task<McpResponse>)Task.FromException(e);
        }
    }

    public Task<McpResponse> GetAsync(in Cuid? id,
        out Wolfpack? result)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id);

            var values = DataCache.Get(id.Value.Value!);
            if (values is not null)
            {
                result = (Wolfpack?)values;
                return Task.FromResult(NotificationAccepted);
            }

            result = null;
            return Task.FromResult(NotificationRejected);
        }
        catch (Exception e)
        {
            result = null;
            return (Task<McpResponse>)Task.FromException(e);
        }
    }

    public Task<McpResponse> RequestListAsync(in Cuid[]? ids,
        out Wolfpack[]? results)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(ids);
            var str = ids.Select(x => x.Value)
                .ToArray();
            var values = DataCache.GetValues(str);
            if (values.Count != 0)
            {
                results = values.Select(x => (Wolfpack)x.Value)
                    .ToArray();
                return Task.FromResult(NotificationAccepted);
            }
            results = null;
            return Task.FromResult(NotificationRejected);
        }
        catch (Exception e)
        {
            results = null;
            return (Task<McpResponse>)Task.FromException(e);
        }

    }

    private static McpResponse NotificationAccepted =>
        new(null,
            $"message: {MessageResponse.Notification}, result: {ResultType.Accepted}",
            null);

    private static McpResponse NotificationRejected =>
        new(null,
            $"message: {MessageResponse.Notification}, result: {ResultType.Rejected}",
            null);

    private static McpResponse ResultAccepted =>
        new(null,
            $"message: {MessageResponse.Notification}, result: {ResultType.Accepted}",
            null);

    private static McpResponse ResultRejected =>
        new(null,
            $"message: {MessageResponse.Notification}, result: {ResultType.Rejected}",
            null);

    private static McpResponse ExceptionThrown(Exception e) =>
        new McpResponse(null,
            $"message: {MessageResponse.Error}, result: {ResultType.Rejected}",
            [
                new McpResource(Cuid.Create(),
                    null,
                    "exception",
                    e.Message,
                    null)
            ]);

    private static McpResponse RequestAccepted =>
        new(null,
            $"message: {MessageResponse.Request}, result: {ResultType.Accepted}",
            null);
}