namespace Direwolf.Dto.Mapper;

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