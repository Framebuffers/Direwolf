using System.Diagnostics;
using System.Text.Json;
using Autodesk.Revit.ApplicationServices;
using Direwolf.Dto.Mapper;
using Direwolf.Dto.Parser;

namespace Direwolf.RevitUI.Hooks;

public partial class EventHooks(ControlledApplication application)
{
    public readonly List<TriggerEventData> Counters = [];
    public readonly List<TimerEventData> Timers = [];
    
    public void LoadTimers()
    {
        // Document Hooks
        MeasureDocumentOpeningTime();
        MeasureSessionTime();
        MeasureDocumentChangeCount();
        MeasureDocumentSavingTime();
    }
}