using Direwolf.Dto.InternalDb;
using Direwolf.Dto.InternalDb.Enums;

namespace Direwolf.Dto.Parser;

public readonly record struct ImportOperation(
    Realm Realm,
    DataType DataType,
    Set MatchCriteria,
    string Value);