using Direwolf.Definitions.Internal.Enums;

namespace Direwolf.Definitions.Telemetry;

// Unimplemented feature as of 2025-05-29
public readonly record struct TimerEventData(
    Method Method,
    EventCondition EventCondition,
    DateTime CreatedAt,
    double IntervalMilliseconds);