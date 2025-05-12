namespace Direwolf.Parsers.Tokens;

/// <summary>
/// The bounds of the set that will be queried and returned as a result.
/// </summary>
public enum QueryScope
{
    Parameter,
    Instance,
    Element,
    ElementType,
    Family,
    Category,
    Document,
    Project
}