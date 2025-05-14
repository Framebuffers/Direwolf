using Direwolf.Dto.InternalDb.Enums;

namespace Direwolf.Dto.Driver;

public abstract record DriverCommonDto(
    string Name,
    (int MajorVersion, int MinorVersion) Version,
    Dictionary<string, string> ParameterList,
    List<CrudOperation> Operations);