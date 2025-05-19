using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;

using Direwolf.Dto.Mapper;
using Direwolf.Sources.InternalDB;

using Application = Autodesk.Revit.ApplicationServices.Application;

namespace Direwolf.RevitUI.Hooks;

public partial class EventHooks
{
    public readonly List<TriggerEventData> Counters = [];
    public readonly List<TimerEventData> Timers = [];
    private readonly ControlledApplication _application;

    public EventHooks(ControlledApplication application)
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