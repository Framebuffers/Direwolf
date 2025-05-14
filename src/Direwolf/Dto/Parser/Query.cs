namespace Direwolf.Dto.Parser;

public readonly record struct Query(
    string Name,
    string Target,
    List<Operation> Operations
);