namespace Direwolf.Dto.Parser;

public readonly record struct Set(
    string Name,
    List<Query> Queries);