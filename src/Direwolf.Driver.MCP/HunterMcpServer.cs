using Anthropic.SDK;
using Direwolf.Definitions.LLM;

namespace Direwolf.Driver.MCP;

public class HunterMcpServer
{
    private static readonly object Lock = new();
    private static HunterMcpServer? _instance;
    private static AnthropicClient? _anthropicClient;

    private HunterMcpServer()
    {
    }

    public static HunterMcpServer GetMcpServer()
    {
        if (_instance is not null) return _instance;
        lock (Lock)
        {
            if (_instance is not null) return _instance;
            _instance = new HunterMcpServer();
        }
        return _instance;
    }
    
    public static void LoadNewClient(string apiKey) => _anthropicClient = new AnthropicClient(apiKey);
    
}

// public class McpToolkit
// {
//     
// }
//
// public class McpCrudSerice : IWolfdenService
// {
//     
// }
//
// public interface IWolfdenService
// {
//     
// }
//
// // tool -> the task itself
// public interface IHunterMcpTool
// {
//     
// }
//
// // resource -> operations to perform a task
// public interface IHunterMcpResource
// {
//     
// }