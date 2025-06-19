using Direwolf.Definitions.Enums;

namespace Direwolf.Definitions.PlatformSpecific.Records;

// Unimplemented feature as of 2025-05-29
public readonly record struct TimerEventData(
    RequestType RequestType,
    EventCondition EventCondition,
    DateTime CreatedAt,
    double IntervalMilliseconds);