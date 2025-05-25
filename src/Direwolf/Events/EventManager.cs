using Autodesk.Revit.ApplicationServices;

using Direwolf.Definitions.Telemetry;

namespace Direwolf.Events;

public partial class EventManager
{
    public readonly List<TriggerEventData> Counters = [];
    public readonly List<TimerEventData> Timers = [];
    private readonly ControlledApplication _application;

    public EventManager(ControlledApplication application)
    {
        _application = application;
    }

    public void LoadTimers()
    {
        // Document Hooks
        OnDocumentOpening();
        OnDocumentClosing();
        MeasureDocumentChangeCount();
        MeasureDocumentSavingTime();
    }
}