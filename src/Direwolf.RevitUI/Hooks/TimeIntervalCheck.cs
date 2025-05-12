namespace Direwolf.RevitUI.Hooks;

internal readonly record struct TimeIntervalCheck(
    OperationType Operation,
    ConditionType Condition,
    DateTime CreatedAt,
    double IntervalMilliseconds
);