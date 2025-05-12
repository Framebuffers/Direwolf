namespace Direwolf.Parsers;

/// <summary>
/// Constrains any given Wolfpack to a given platform. Some features cannot run on ACC, therefore they are
/// validated ahead of time.
/// </summary>
public enum PlatformType
{
    Other,
    Desktop,
    Cloud,
}