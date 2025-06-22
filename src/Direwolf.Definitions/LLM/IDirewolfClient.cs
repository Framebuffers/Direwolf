namespace Direwolf.Definitions.LLM;

/// <summary>
/// Operations in Direwolf are performed by shallow-copying the same Wolfpack back, with the
/// requested data 
/// </summary>
public interface IDirewolfClient
{
    Task<Wolfpack> CreateAsync(in WolfpackMessage wolfpackMessage);
    Task<Wolfpack> UpdateAsync(in WolfpackMessage wolfpackMessage);
    Task<Wolfpack> DeleteAsync(in WolfpackMessage wolfpackMessage);
    Task<Wolfpack> GetAsync(in WolfpackMessage wolfpackMessage);
    Task<Wolfpack> GetManyAsync(in WolfpackMessage wolfpackMessage);
}