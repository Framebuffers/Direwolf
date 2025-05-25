using Direwolf.Definitions.Internal.Enums;

namespace Direwolf.Definitions.Telemetry;

public readonly record struct TriggerEventData(
    Realm          Realm,
    EventCondition EventCondition,
    DateTime       CreatedAt,
    bool           TriggerResult = true);