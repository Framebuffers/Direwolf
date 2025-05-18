using Direwolf.Dto.InternalDb;

namespace Direwolf.Dto.Parser;

public readonly record struct Query(
    string Name,
    string Target,
    Dictionary<string, InternalDb.Transaction> Transactions
);