using Direwolf.Dto.InternalDb.Enums;

namespace Direwolf.Dto.Parser;

public readonly struct Operation(
    Realm Scope,
    string P,
    LogicalOperator OP,
    string Q);