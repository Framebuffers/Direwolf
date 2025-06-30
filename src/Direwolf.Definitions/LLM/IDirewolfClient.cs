namespace Direwolf.Definitions.LLM;

/// <summary>
/// Operations in Direwolf are performed by shallow-copying the same Wolfpack back, with the
/// requested data 
/// </summary>
public interface IDirewolfClient
{
    Task<WolfpackMessage> CreateAsync(in WolfpackMessage wolfpackMessage);
    Task<WolfpackMessage> UpdateAsync(in WolfpackMessage wolfpackMessage);
    Task<WolfpackMessage> DeleteAsync(in WolfpackMessage wolfpackMessage);
    Task<WolfpackMessage> GetAsync(in WolfpackMessage wolfpackMessage);
    Task<WolfpackMessage> GetManyAsync(in WolfpackMessage wolfpackMessage);
}