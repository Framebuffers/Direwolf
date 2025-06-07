namespace Direwolf.Definitions.Telemetry;

// Unimplemented feature as of 2025-05-29
/// <summary>
///     Trigger to execute any given query.
/// </summary>
public enum ExecutionTrigger
{
    None,
    OnEvent,
    True,
    False,
    Null
}