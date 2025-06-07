using Direwolf.Definitions.Internal;

namespace Direwolf.Definitions.Parser;

// Unimplemented feature as of 2025-05-29
public readonly record struct Query(string Name, string Target, Dictionary<string, Transaction> Transactions);