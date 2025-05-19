using Direwolf.Dto.InternalDb.Enums;

namespace Direwolf.Dto.Parser;

public readonly record struct Target(Realm Realm, TargetType Type, string Name, string[] Arguments);