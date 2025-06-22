using System.Text.Json;
using Direwolf.Definitions.LLM;
using Direwolf.Driver.MCP.Tools;

namespace Direwolf.Driver.MCP;

public class HunterClient(MCPDriver driver, TextReader? input = null, TextWriter? output = null)
{
    public async Task RunAsync()
    {
        string line;
        while ((line = await input.ReadLineAsync()) is not null)
        {
            try
            {
                var request = JsonSerializer.Deserialize<WolfpackMessage>(line);
                var response = await driver.HandleRequest(request);
                var responseJson = JsonSerializer.Serialize(response);
                await output?.WriteLineAsync(responseJson)!;
                await output?.FlushAsync()!;
            }
            catch (Exception e)
            {
                var error = OperationHandler.CreateErrorResponse(-32700, "Parsing error.", e.Message);
                var json = JsonSerializer.Serialize(error);
                await output?.WriteLineAsync(json)!;
                await output?.FlushAsync()!;
                throw;
            }
        }
    }
}