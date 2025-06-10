using Direwolf.Definitions.Internal.Enums;

namespace Direwolf.Definitions.Parser;

// Unimplemented feature as of 2025-05-29
public readonly record struct Operation(Method Scope, string P, LogicalOperator OP, string Q);