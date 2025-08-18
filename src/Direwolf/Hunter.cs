using System.Runtime.Caching;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.LLM;

namespace Direwolf;

public class Hunter : IHunter
{
    private static readonly object Lock = new();
    private static Hunter? _instance;
    private static Direwolf? _direwolf;
    private static readonly CacheItemPolicy _policy = new CacheItemPolicy(){ SlidingExpiration = TimeSpan.FromMinutes(60) };
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

    public Task<WolfpackMessage> CreateAsync(in WolfpackMessage wolfpackMessage)
    {
        if (wolfpackMessage.Parameters is null) throw new ArgumentNullException(nameof(wolfpackMessage));
        var parameters = (Dictionary<string, object>)wolfpackMessage.Parameters;
        var newElements = (Dictionary<string, object>)parameters.Values.First();
        foreach (var el in newElements)
        {
            Wolfden.GetDatabase().Add(el.Key, el.Value);
        }

        var result = wolfpackMessage with
        {
            MessageType = MessageResponse.Result.ToString(),
            Result = new Dictionary<string, object> { ["create"] = "ok" }
        };
        
        return Task.FromResult(result);
    }

    public Task<WolfpackMessage> UpdateAsync(in WolfpackMessage wolfpackMessage)
    {
         if (wolfpackMessage.Parameters is null) throw new ArgumentNullException(nameof(wolfpackMessage));
         var parameters = (Dictionary<string, object>)wolfpackMessage.Parameters;
         var newElements = (Dictionary<string, object>)parameters.Values.First();
         
         foreach (var el in newElements)
         {
             var db = Wolfden.GetDatabase();
             if (!db.TryGetValue(el.Key, out var value))
             {
                 db[el.Key] = el.Value;
             }
         }

         var result = wolfpackMessage with
         {
             MessageType = MessageResponse.Result.ToString(),
             Result = new Dictionary<string, object> { ["create"] = "ok" }
         };
        
         return Task.FromResult(result);
    }

    public Task<WolfpackMessage> DeleteAsync(in WolfpackMessage wolfpackMessage)
    {
        throw new NotImplementedException();
    }

    public Task<WolfpackMessage> GetAsync(in WolfpackMessage wolfpackMessage)
    {
        throw new NotImplementedException();
    }

    public Task<WolfpackMessage> ListAsync(int limit = 100, int offset = 0)
    {
        throw new NotImplementedException();
    }
}