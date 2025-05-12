namespace Direwolf.Parsers.Tokens;

public readonly struct Operation(
    QueryScope Scope,
    string P,
    LogicalOperator OP,
    string Q);