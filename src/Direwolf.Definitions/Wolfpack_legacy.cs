// using System.Net;
// using System.Reflection;
// using System.Runtime.Caching;
// using System.Text.Json;
// using System.Text.Json.Serialization;
// using Direwolf.Definitions.Enums;
// using Direwolf.Definitions.Extensions;
// using Direwolf.Definitions.LLM;
// using Direwolf.Definitions.Serialization;
//
// namespace Direwolf.Definitions;
//
// /*
//  * Now that I introduced MCP to the mix, I realized that Wolfpack is too bloated of a token to be passed around.
//  * MCP needs, basically, the data from Wolfpack from start to finish. it'd make life easier if these two could merge into one.
//  * So, this is the plan:
//  *  - payload moves to WolfpackMessage
//  *  - howl and wolfpack merge to only have one set of data-- this way, the shallow-copy method is actually real.
//  *    yay another refactor that'll delay this shit just cause I want it to integrate with LLM's.
//  *  - *please* fix the formatting with properties.
//  *  - merge the logic between Wolfden and Direwolf.
//  *      - rn direwolf is kinda useless. all the cool stuff is done by Wolfden. However, the idea is that
//  *        direwolf deals with the **specific** implementation. Should be the other way around but dunno if it's worth
//  *        changing at this point. either way, decoupling these two is a good idea *in paper*.
//  *      - direwolf handles all the revit shit -> wolfden is dumb and just takes the WolfpackMessage payload straight as a
//  *        CacheElement (so, Dictionary<string, object>) and uses *that* as a key.
//  *      - if I wanna use the howl's ID as key, use that on the payload. but the Wolfpack doesn't care about what's inside.
//  *        it's only a supermarket trolley.
//  *      - *if* possible, change the Result enum to a standard HTTP code (probably from a .NET lib).
//  *      - There are two wolves inside you. one is CRUD/RESTful and the other is JSON-RPC. Wolfden is pure REST, only processes
//  *        exposes HTTP-like endpoints and uses the Response record to say what happened. if a, for example, 404 happens,
//  *        this is the way dwolf *knows* it didn't find the object, not a custom enum. It truly speaks Web.
//  *      - Hunter, on the other hand, is fully JSON-RPC. This is how it works:
//  *              - Howls are JSON-RPC2.0 messages. They have a JSON-RPC2.0 header.
//  *              - Hunter exposes an API to an MCP client with HTTP RESTful basic operations based on HTTP: get (read),
//  *                get[](read, but many), post(update), put(add) [might merge with post], delete (probably add more HTTP calls).
//  *              - Any client that wants to extend Hunter, can do whatever it wants: move wolfpacks back and forth, whatever --they all have the same ID.
//  *              - The *message* is JSON-RPC, the API is REST. why? it's a database. REST is CRUD after all, it's what databases do.
//  *      - REST URI's:
//  *              - this one is also crucial: every single element must have its own URI. How to do this quick?
//  *                use ForgeTypeId (which is what it's meant for lol) as a URI descriptor. ez
//  *      - MCP:
//  *              - shapes the structure of Wolfpacks.
//  *              - mcp is JSON-RPC2.0 with extra steps. wolfpack is mcp with extra steps. it just uses the same fields for something else.
//  *      - LLM integration:
//  *              - now this one is spicy.
//  *              - each client, just as expected, is in charge of its own templates and prompts. don't work that aisle.
//  *              - *however*, direwolf injects its own "header prompt" to make it a wolfpack.
//  *              - the other spicy thing: hunter only caches context until it's flushed. it's all RAM-based, and meant to be flushed after 1 hour.
//  *                  - why? speed. this baby runs super fast because it's all RAM-based. have to check out the best approach for this, but
//  *                    my theory of operation, is that it learns by itself and uses the API key to identify you. it's gonna learn commands the more
//  *                    it's being used, and in case you switch clients, I store the context to bring it up. probably gonna have to roll a MongoDB but whatever, problem for the Frame of tomorrow.
//  *              - the one in charge is Hunter. It works the *external requests* aisle. if it talks in LLM or in local Wolfpack,
//  *                doesn't really matter. it only exposes, literally, async CRUD operations. if you want to do the fancy LLM shit,
//  *                use your own driver (in this case, Direwolf.Driver.MCP). Just happens that this one is pretty lean cause
//  *                MCP is Direwolf's mother tongue.
//  *      - Revit?
//  *              - the idea is to do a simple (1) demo: take all the elements on screen (or a selected one) and write any prompt with it.
//  *                  - direwolf holds the cache for all elements on screen. more efficient anyways.
//  *                  - the command sends the UUID of all elements selected and the prompt.
//  *                  - direwolf sends the RevitElement as a JSONL stiring with only UUID and parameters, a Dictionary of attributes.
//  *                  - direwolf injects this prompt:
//  *                      "you are an AI, using the Wolfpack {wpack version} protocol.
//  *                      you work using the {Revit Version} application's Autodesk.Revit.DB.Element format
//  *                      you have been tasked with {prompt}
//  *                      use this data to fulfill this request
//  *                      create a Revit Transaction that creates a Revit Schedule with the results, following the given API
//  *                      create an artifact that exports the file to CSV and send it back."
//  *                  - ...or something like that. open for comments.
//  *                  - the whole demo depends on this, it better work cause I am suffering trying to finish this.
//  *      - Documentation:
//  *              _ at the *very end*, update the XLS docs, update the writeup and launch.
//  *
//  *  it's gonna be alright frame, just do it. we counting on ya <3
//  */
//
//
// //TODO: move payload to Properties and everything pointing to it. it is now inside the Properties.
// public readonly record struct Wolfpack(
//     [property: JsonPropertyName("id")]            Cuid Id, 
//     [property: JsonPropertyName("name")]          string? Name, 
//     [property: JsonPropertyName("description")]   string? Description,
//     [property: JsonIgnore]                        HttpMethod Method, //   direwolf-specific
//     [property: JsonPropertyName("@Properties")]       WolfpackMessage Params )
// {
//     [JsonPropertyName("jsonrpc"), JsonPropertyOrder(0)] public const string JsonRpc = "2.0";
//     [JsonPropertyName("wolfpack")] public const string WolfpackVersion = "1.0";
//     [JsonPropertyName("createdAt")] public DateTimeOffset? CreatedAt => Id.GetDateTimeCreation();
//     [JsonPropertyName("updatedAt")] public DateTime UpdatedAt => DateTime.UtcNow;
//     
//     public static Wolfpack Create(Cuid? id, 
//         string? name,
//         string? description,
//         HttpMethod requestType, 
//         WolfpackMessage @Properties)
//     {
//         return new Wolfpack(id ?? Cuid.Create(),
//             name, 
//             description,
//             requestType, 
//             @Properties);
//     }
//    
//
//     // public static Wolfpack AsPrompt(Wolfpack w, 
//     //     string promptName, 
//     //     string promptDescription, 
//     //     DataType payloadType,
//     //     Wolfpack[]? data,
//     //     string uri)
//     // {
//     //     var args = new WolfpackMessage(promptName, 
//     //         promptDescription, 
//     //         payloadType.ToString(), 
//     //         1, 
//     //         MessageResponse.Request.ToString(),
//     //         ResultType.Rejected.ToString(), 
//     //         uri);
//     //     
//     //     var wp = new Wolfpack(Cuid.Create(), 
//     //         MessageType.Get, 
//     //         promptName, 
//     //         args, 
//     //         data?.Select(McpResourceContainer.Create)
//     //             .ToArray(),
//     //         promptDescription);
//     //     return wp;
//     // }
//
//     public CacheItem AsCacheItem() => new CacheItem(Id.Value, this);
// }
