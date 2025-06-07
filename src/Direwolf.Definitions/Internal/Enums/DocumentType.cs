namespace Direwolf.Definitions.Internal.Enums;

/// <summary>
///     Constrains the execution of this query to a given type of Document. This is
///     used to filter out some operations
///     that are valid only in one or another.
/// </summary>
public enum DocumentType
{
    Project,
    Family
}