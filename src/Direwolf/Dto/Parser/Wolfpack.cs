using Direwolf.Drivers;
using Direwolf.Dto.InternalDb.Enums;

namespace Direwolf.Dto.Parser;
public record Wolfpack(
    string Name,
    WolfpackParameters Parameters,
    List<Query> Queries,
    List<Driver> Drivers
);