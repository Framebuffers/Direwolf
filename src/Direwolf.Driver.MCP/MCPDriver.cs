using System.Runtime.Caching;
using Anthropic.SDK;
using Direwolf.Definitions;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.LLM;
using Direwolf.Driver.MCP.Tools;

namespace Direwolf.Driver.MCP;

//TODO: connect it to Hunter on the Direwolf side.
/// <summary>
/// Acts as the MCP driver for Direwolf. MCP drivers provide context, tools, prompts and utilities to communicate
/// with other LLMs that implement this protocol. <see cref="Direwolf"/> drivers are assemblies that use the Wolfpack format
/// to communicate and transform data. This driver provides access to the Direwolf context, templates to communicate with
/// BIM applications and methods to pipe information back to the client through the Host.
/// </summary>
public sealed partial class MCPDriver
{
    /// <summary>
    /// Reference to a Direwolf instance,
    /// </summary>
    private static Hunter? _hunter;
    private static AnthropicClient? _anthropicClient;
    private static MCPDriver? _instance;
    private static readonly object Lock = new();
    private readonly Dictionary<string, Func<object, Task<object>>> _handlers = new();  
    
    private MCPDriver(Hunter hunter)
    {
        _hunter = hunter;
    }

    public static MCPDriver GetInstance(Hunter hunter, string anthropicApiKey)
    {
        if (_instance is not null) return _instance;
        lock (Lock)
        {
            if (_instance is not null) return _instance;
            _instance = new MCPDriver(hunter);
            _anthropicClient = new AnthropicClient(anthropicApiKey);
            return _instance;
        }
    }
    
}
//
// public sealed partial class MCPDriver
// {
//     //TODO: Create handlers
//     private Dictionary<string, Func<object, Task<object>>> InitToolHandlers()
//     {
//         return new()
//         {
//             ["create_wolfpack"] = ToolFactory.CreateWolfpack,
//             
//         }
//     }
// }