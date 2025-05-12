namespace Direwolf.RevitUI.Hooks;

internal enum ConditionType
{
    OnProgress = 0,
    OnExecution = 1,
    OnOpening = 2,
    OnClosing = 3,
    OnModifying = 4,
    OnDeleting = 5,
    OnSaving = 6,
    OnSavingAs = 7,
    OnExporting = 8,
    OnCreating = 9,
    OnSync = 10,
    OnReload = 11
}