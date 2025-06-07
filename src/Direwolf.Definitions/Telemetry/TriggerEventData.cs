using Direwolf.Definitions.Internal.Enums;

namespace Direwolf.Definitions.Telemetry;

// Unimplemented feature as of 2025-05-29
public readonly record struct TriggerEventData(
    Realm Realm,
    EventCondition EventCondition,
    DateTime CreatedAt,
    bool TriggerResult = true);