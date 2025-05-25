using Direwolf.Definitions.Internal.Enums;

namespace Direwolf.Definitions.Parser;

public readonly record struct Operation(
    Realm           Scope,
    string          P,
    LogicalOperator OP,
    string          Q);