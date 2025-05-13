namespace Direwolf.Parsers.Tokens;

/// <summary>
/// Trigger to execute any given query.
/// </summary>
public enum ExecutionTrigger
{
    None,
    OnEvent,
    True,
    False,
    Null
}