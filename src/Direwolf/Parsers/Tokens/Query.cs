namespace Direwolf.Parsers.Tokens;

public readonly record struct Query(
    string Name,
    string Target,
    List<Operation> Operations
);