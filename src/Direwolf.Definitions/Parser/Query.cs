using Direwolf.Definitions.Internal;

namespace Direwolf.Definitions.Parser;

public readonly record struct Query(
    string                          Name,
    string                          Target,
    Dictionary<string, Transaction> Transactions);