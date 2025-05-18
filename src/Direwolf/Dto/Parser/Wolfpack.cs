namespace Direwolf.Dto.Parser;

public record Wolfpack(
    string Name,
    WolfpackParameters Parameters,
    List<Query> Queries,
    List<Driver> Drivers
);