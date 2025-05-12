namespace Direwolf.Parsers.Tokens;

public readonly record struct SetOperation(
    string Name,
    Set P,
    LogicalOperator OP,
    Set Q);