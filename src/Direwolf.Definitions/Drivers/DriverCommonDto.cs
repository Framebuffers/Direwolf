using Direwolf.Definitions.Internal.Enums;

namespace Direwolf.Definitions.Drivers;

public abstract record DriverCommonDto(
    string Name,
    (int MajorVersion, int MinorVersion) Version,
    Dictionary<string, string> ParameterList,
    List<CrudOperation> Operations);