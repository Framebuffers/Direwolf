using Direwolf.Definitions.Internal.Enums;

namespace Direwolf.Definitions.Parser;

public readonly record struct Target(
    Realm      Realm,
    TargetType Type,
    string     Name,
    string[]   Arguments);