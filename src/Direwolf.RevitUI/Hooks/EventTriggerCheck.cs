namespace Direwolf.RevitUI.Hooks;

internal readonly record struct EventTriggerCheck(
    OperationType Operation,
    ConditionType Condition,
    DateTime CreatedAt,
    bool TriggerResult = true
);