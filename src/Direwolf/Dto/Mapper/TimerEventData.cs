using Direwolf.Dto.InternalDb.Enums;

namespace Direwolf.Dto.Mapper;

public readonly record struct TimerEventData(
    Realm          Realm,
    EventCondition EventCondition,
    DateTime       CreatedAt,
    double         IntervalMilliseconds);