namespace Direwolf.Definitions.LLM;

/// <summary>
/// Operations in Direwolf are performed by shallow-copying the same Wolfpack back, with the
/// requested data 
/// </summary>
public interface IHunter
{
    /// <summary>
    /// Must declare inside Parameters:
    /// <list type="bullet">
    ///     <item>[create] = { [item_key] = item_value }</item>
    /// </list>
    /// </summary>
    /// <param name="wolfpackMessage"></param>
    /// <returns></returns>
    Task<WolfpackMessage> CreateAsync(in WolfpackMessage wolfpackMessage);
    
    /// <summary>
    /// Must declare inside Parameters:
    /// <list type="bullet">
    ///     <item>[key_to_update] = new_values</item>
    /// </list>
    /// </summary>
    /// <param name="wolfpackMessages"></param>
    /// <returns></returns>
    Task<WolfpackMessage> UpdateAsync(in WolfpackMessage wolfpackMessages);

    /// <summary>
    /// Must declare inside Parameters:
    /// <list type="bullet">
    ///     <item>[delete] = Array[keys_to_delete]</item>
    /// </list>
    /// </summary>
    /// <param name="wolfpackMessage"></param>
    /// <returns></returns>
    Task<WolfpackMessage> DeleteAsync(in WolfpackMessage wolfpackMessage);
    
    /// <summary>
    /// Must declare inside Parameters:
    /// <list type="bullet">
    ///     <item>[get] = Array[keys_to_get]</item>
    /// </list>
    /// </summary>
    /// <param name="wolfpackMessage"></param>
    /// <returns></returns>
    Task<WolfpackMessage> GetAsync(in WolfpackMessage wolfpackMessage);
    
    /// <summary>
    /// Must declare inside Parameters:
    /// <list type="bullet">
    ///     <item>[list] = { [limit] = item_limit, [offset] = offset_number }</item>
    /// </list>
    /// </summary>
    /// <param name="limit"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    Task<WolfpackMessage> ListAsync(int limit = 100, int offset = 0);
}