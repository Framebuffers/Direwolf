// using System.Runtime.Caching;
// using Direwolf.Definitions;
// using Direwolf.Definitions.LLM;
//
// namespace Direwolf;
//
// //TODO: refactor to use Wolfpack for everything **except** finalised data out.
// public sealed class Hunter : IHunter
// {
//     private static readonly object Lock = new();
//     private static Hunter? _instance;
//     private static Direwolf? _direwolf;
//     private static readonly ObjectCache DataCache = MemoryCache.Default; // private cache
//     private static ObjectCache? Wolfden => _direwolf?.GetHunterCache();
//     private static readonly CacheItemPolicy Policy = new() { SlidingExpiration = TimeSpan.FromMinutes(60) };
//     public const string McpProtocolVersion = "2025-06-18";
//     public static int CacheCount = DataCache.Count();
//
//     private Hunter(Direwolf? direwolf)
//     {
//         _direwolf = direwolf;
//     }
//
//     public static Hunter GetInstance(Direwolf direwolf)
//     {
//         if (_instance is not null) return _instance;
//         lock (Lock)
//         {
//             if (_instance is not null) return _instance;
//             _instance = new Hunter(direwolf);
//             return _instance;
//         }
//     }
//
//     public static async Task<bool> Import()
//     {
//         if (_direwolf is null) throw new NullReferenceException();
//         foreach (var element in Wolfden!)
//         {
//             DataCache.Add(new CacheItem(element.Key, element.Value), Policy);
//         }
//
//         return await Task.FromResult(true);
//     }
//
//     public static async Task<bool> Export()
//     {
//         if (Wolfden is null) throw new NullReferenceException();
//         foreach (var element in DataCache)
//         {
//             Wolfden.Add(new CacheItem(element.Key, element.Value), Policy);
//         }
//
//         return await Task.FromResult(true);
//     }
//
//     /// <summary>
//     /// When a Wolfpack is created, it will strip the <see cref="WolfpackMessage.Parameters"/> dictionary's data
//     /// and do two things: create a CacheElement, delete the entry from the dictionary.
//     /// Whenever a Wolfpack is returned with an empty Payload, means it loaded all elements to Cache. 
//     /// </summary>
//     /// <param name="wolfpackMessage"></param>
//     /// <returns></returns>
//     public Task<WolfpackMessage> CreateAsync(in WolfpackMessage wolfpackMessage)
//     {
//         try
//         {
//             ArgumentNullException.ThrowIfNull(wolfpackMessage.Parameters);
//             Crud(in wolfpackMessage, Operation.Create, out var wolfpack);
//             return Task.FromResult(wolfpack);
//         }
//         catch (Exception e)
//         {
//             return (Task<WolfpackMessage>)Task.FromException(e);
//         }
//     }
//
//     /// <summary>
//     /// Takes the keys inside <see cref="WolfpackMessage.Parameters"/> and looks for them inside the <see cref="DataCache"/>.
//     /// If found, it will remove the element with the same key, and add another with the values stored in Parameters.
//     /// <remarks>
//     /// All operations are one-on-one with <see cref="WolfpackMessage.Parameters"/>: all key-value pair inside the <see cref="WolfpackMessage"/>
//     /// equals a key-value pair inside <see cref="DataCache"/>. Use this property to manipulate the database directly.
//     /// </remarks>
//     /// </summary>
//     /// <param name="wolfpackMessage"></param>
//     /// <returns></returns>
//     public Task<WolfpackMessage> UpdateAsync(in WolfpackMessage wolfpackMessage)
//     {
//         try
//         {
//             ArgumentNullException.ThrowIfNull(wolfpackMessage.Parameters);
//             Crud(in wolfpackMessage, Operation.Update, out var wolfpack);
//             return Task.FromResult(wolfpack);
//         }
//         catch (Exception e)
//         {
//             return (Task<WolfpackMessage>)Task.FromException(e);
//         }
//     }
//
//     /// <summary>
//     /// Looks for all cached elements with the same keys as the ones inside <see cref="WolfpackMessage.Parameters"/> and
//     /// deletes them from the <see cref="DataCache"/>.
//     /// </summary>
//     /// <param name="wolfpackMessage"></param>
//     /// <returns></returns>
//     public Task<WolfpackMessage> DeleteAsync(in WolfpackMessage wolfpackMessage)
//     {
//         try
//         {
//             ArgumentNullException.ThrowIfNull(wolfpackMessage.Parameters);
//             Crud(in wolfpackMessage, Operation.Delete, out var wolfpack);
//             return Task.FromResult(wolfpack);
//         }
//         catch (Exception e)
//         {
//             return (Task<WolfpackMessage>)Task.FromException(e);
//         }
//     }
//
//     /// <summary>
//     /// Looks for all the cached elements matching all keys inside <see cref="WolfpackMessage.Parameters"/>, and
//     /// returns a Wolfpack with the values filled.
//     ///
//     /// <remarks>
//     /// To read values, add the keys to be read to the dictionary and leave their value null. If successful, all found
//     /// values will be paired to their requested key.
//     /// </remarks>
//     /// </summary>
//     /// <param name="wolfpackMessage"></param>
//     /// <returns></returns>
//     public Task<WolfpackMessage> GetAsync(in WolfpackMessage wolfpackMessage)
//     {
//         try
//         {
//             ArgumentNullException.ThrowIfNull(wolfpackMessage.Parameters);
//             var decode = (IDictionary<string, object>)wolfpackMessage.Parameters;
//
//             var addedPayload = wolfpackMessage with { Parameters = decode["keys"] };
//             Crud(in addedPayload, Operation.Read, out var wolfpack);
//             return Task.FromResult(wolfpack);
//         }
//         catch (Exception e)
//         {
//             return (Task<WolfpackMessage>)Task.FromException(e);
//         }
//     }
//
//     /// <inheritdoc cref="GetAsync"/>>
//     /// <inheritdoc/>>
//     /// <param name="wolfpackMessage"></param>
//     /// <returns></returns>
//     public Task<WolfpackMessage> GetManyAsync(in WolfpackMessage wolfpackMessage)
//     {
//         try
//         {
//             Crud(in wolfpackMessage, Operation.ReadArray, out var wolfpack);
//             return Task.FromResult(wolfpack);
//         }
//         catch (Exception e)
//         {
//             return (Task<WolfpackMessage>)Task.FromException(e);
//         }
//     }
//
//     private enum Operation
//     {
//         Create,
//         Read,
//         ReadArray,
//         Update,
//         Delete
//     }
//
//     private void Crud(in McpRequest input, out McpRequest output, string? key)
//     {
//
//         if (input is null) throw new NullReferenceException();
//         key ??= Cuid.Create().Value;
//         
//         var counter = 0;
//
//             var cacheKey = new CacheItem(key, input.);
//             switch (input.Method)
//             {
//                 case "create":
//                     DataCache.Add(new CacheItem(key, incomingDictionary[incomingKey.Key]), Policy);
//                     counter++;
//                     break;
//                 case "read":
//                     var readValue = DataCache.Get(cacheKey.Key);
//                     output = input with { s = readValue };
//                     counter++;
//                     break;ggVG
//                 case "read_many":
//                     var values = DataCache.GetValues(incomingDictionary.Keys);
//                     output = input with { Result = values };
//                     break;
//                 case "update":
//                     DataCache.Remove(key);
//                     DataCache.Add(cacheKey, Policy);
//                     counter++;
//                     break;
//                 case "delete":
//                     DataCache.Remove(key);
//                     counter++;
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException(nameof(input), input, null);
//             
//         }
//
//         output = input with
//         {
//             Parameters = new
//             {
//                 updated = counter
//             }
//         };
//     }
//
//
//     public Task<McpResponse> CreateAsync(in McpRequest wolfpackMessage)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Task<McpResponse> UpdateAsync(in McpRequest wolfpackMessage)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Task<McpResponse> DeleteAsync(in McpRequest wolfpackMessage)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Task<McpResponse> GetAsync(in McpRequest wolfpackMessage)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Task<McpResponse> GetManyAsync(in McpRequest wolfpackMessage)
//     {
//         throw new NotImplementedException();
//     }
// }