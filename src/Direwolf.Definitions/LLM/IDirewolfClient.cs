namespace Direwolf.Definitions.LLM;

/// <summary>
/// Operations in Direwolf are performed by shallow-copying the same Wolfpack back, with the
/// requested data 
/// </summary>
public interface IDirewolfClient
{
    Task<McpResponse> CreateAsync(in WolfpackParams parameters, out Wolfpack? wolfpack);
    Task<McpResponse> UpdateAsync(in Wolfpack? updateArgs);
    Task<McpResponse> DeleteAsync(in Cuid keys);
    Task<McpResponse> GetAsync(in Cuid? id, out Wolfpack? result);
    Task<McpResponse> RequestListAsync(in Cuid[]? ids, out Wolfpack[]? results);
}