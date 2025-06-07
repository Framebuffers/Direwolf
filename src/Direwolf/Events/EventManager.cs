using Autodesk.Revit.ApplicationServices;
using Direwolf.Definitions.Telemetry;

namespace Direwolf.Events;

// Unimplemented feature as of 2025-05-29
public partial class EventManager(ControlledApplication application)
{
    private readonly List<TriggerEventData> Counters = [];

    private readonly List<TimerEventData> Timers = [];

    public void LoadTimers()
    {
        OnDocumentOpening();
        OnDocumentClosing();
        MeasureDocumentChangeCount();
        MeasureDocumentSavingTime();
    }
}