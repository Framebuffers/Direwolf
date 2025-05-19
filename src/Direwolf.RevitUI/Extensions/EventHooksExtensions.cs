using System.Diagnostics;
using System.Text.Json;

using Direwolf.Dto.Mapper;
using Direwolf.RevitUI.Hooks;

namespace Direwolf.RevitUI.Extensions;

public static class EventHooksExtensions
{
    public static void Debug_PrintContentsOfCheckRegistry(this EventHooks e)
    {
        using StringWriter sw = new();
        SortedList<DateTime, TimerEventData> sortedList = [];
        foreach (var eventData in e.Timers)
            sortedList.Add(eventData.CreatedAt,
                           eventData);
        Debug.Print(JsonSerializer.Serialize(sortedList));
    }

    public static void Debug_PrintContentsOfTriggerRegistry(this EventHooks e)
    {
        using StringWriter sw = new();
        SortedList<DateTime, TriggerEventData> sortedList = [];
        foreach (var eventData in e.Counters)
            sortedList.Add(eventData.CreatedAt,
                           eventData);
        Debug.Print(JsonSerializer.Serialize(sortedList));
    }
}