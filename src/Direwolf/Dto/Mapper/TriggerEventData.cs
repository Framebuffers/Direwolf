using Direwolf.Dto.InternalDb.Enums;
using Direwolf.Dto.Parser;

namespace Direwolf.Dto.Mapper;

public readonly record struct TriggerEventData(
    Realm Realm,
    EventCondition EventCondition,
    DateTime CreatedAt,
    bool TriggerResult = true
);