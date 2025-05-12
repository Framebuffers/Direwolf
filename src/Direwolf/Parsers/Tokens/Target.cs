namespace Direwolf.Parsers.Tokens;

public readonly record struct Target(
    string Name,
    TargetType Type,
    string Destination
);