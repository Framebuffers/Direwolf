namespace Direwolf.Parsers.Tokens;

public readonly record struct Set(
    string Name,
    List<Query> Queries);