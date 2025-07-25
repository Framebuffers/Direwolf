namespace Direwolf.Definitions.LLM;

/// <summary>
/// Operations in Direwolf are performed by shallow-copying the same Wolfpack back, with the
/// requested data 
/// </summary>
public interface IDirewolfClient
{
    Task<McpResponse> CreateAsync(in McpRequest wolfpackMessage);
    Task<McpResponse> UpdateAsync(in McpRequest wolfpackMessage);
    Task<McpResponse> DeleteAsync(in McpRequest wolfpackMessage);
    Task<McpResponse> GetAsync(in McpRequest wolfpackMessage);
    Task<McpResponse> GetManyAsync(in McpRequest wolfpackMessage);
}