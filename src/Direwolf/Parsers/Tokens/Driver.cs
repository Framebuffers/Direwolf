namespace Direwolf.Parsers.Tokens;

public readonly record struct Driver(
    string Name,
    (int MajorVersion, int MinorVersion) Version,
    Dictionary<string, string> ParameterList,
    List<CrudOperation> Operations);