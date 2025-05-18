using Direwolf.Dto.InternalDb.Enums;

namespace Direwolf.Dto.Mapper;

public readonly record struct TriggerEventData(
    Realm Realm,
    EventCondition EventCondition,
    DateTime CreatedAt,
    bool TriggerResult = true
);