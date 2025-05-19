namespace Direwolf.Dto.Parser;

public readonly record struct Driver(string Name, Dictionary<string, string> Parameters);