using System.Diagnostics;
using Direwolf.Dto.InternalDb.Enums;
using Direwolf.Dto.Mapper;
using Direwolf.Dto.Parser;

namespace Direwolf.RevitUI.Hooks;

public partial class EventHooks
{
    private readonly Stopwatch _direwolfQueryOperationTimer = new();
    private readonly Stopwatch _direwolfSetOperationTimer = new();
    private readonly Stopwatch _direwolfInternalOperationTimer = new();
    private readonly Stopwatch _parameterOperationTimer = new();
    private readonly Stopwatch _instancesOperationTimer = new();
    private readonly Stopwatch _elementOperationTimer = new();
    private readonly Stopwatch _elementTypeOperationTimer = new();
    private readonly Stopwatch _groupOperationTimer = new();
    private readonly Stopwatch _viewOperationTimer = new();
    private readonly Stopwatch _scheduleOperationTimer = new();
    private readonly Stopwatch _linkOperationTimer = new();
    private readonly Stopwatch _familyOperationTimer = new();
    private readonly Stopwatch _categoryOperationTimer = new();
    private readonly Stopwatch _documentOperationTimer = new();
    private readonly Stopwatch _revitDatabaseOperationTimer = new();
    private readonly Stopwatch _fileOperationTimer = new();
    private readonly Stopwatch _localWorkshareOperationTimer = new();
    private readonly Stopwatch _cloudWorkshareOperationTimer = new();
    private readonly Stopwatch _dataWarehouseOperationTimer = new();
    private readonly Stopwatch _worksharingOperationTimer = new();
    private readonly Stopwatch _revitUIApplicationOperationTimer = new();

    private void AddTimeIntervalCheck(Realm op, EventCondition eventCondition)
    {
        switch (op)
        {
            case Realm.DirewolfQuery:
                _direwolfQueryOperationTimer.Stop();
                Timers.Add(new TimerEventData(op, eventCondition, DateTime.Now,
                    _direwolfQueryOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print(
                    $"\n\nDirewolfQuery operation completed. Result = {Timers.Last().ToString()}\n\n");
                _direwolfQueryOperationTimer.Restart();
                break;
            case Realm.DirewolfSet:
                _direwolfSetOperationTimer.Stop();
                Timers.Add(new TimerEventData(op, eventCondition, DateTime.Now,
                    _direwolfSetOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print(
                    $"\n\nDirewolfSet operation completed. Result = {Timers.Last().ToString()}\n\n");
                _direwolfSetOperationTimer.Restart();
                break;
            case Realm.DirewolfInternal:
                _direwolfInternalOperationTimer.Stop();
                Timers.Add(new TimerEventData(op, eventCondition, DateTime.Now,
                    _direwolfInternalOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print(
                    $"\n\nDirewolfInternal operation completed. Result = {Timers.Last().ToString()}\n\n");
                _direwolfInternalOperationTimer.Restart();
                break;
            case Realm.Parameter:
                _parameterOperationTimer.Stop();
                Timers.Add(new TimerEventData(op, eventCondition, DateTime.Now,
                    _parameterOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print($"\n\nParameter operation completed. Result = {Timers.Last().ToString()}\n\n");
                _parameterOperationTimer.Restart();
                break;
            case Realm.Instances:
                _instancesOperationTimer.Stop();
                Timers.Add(new TimerEventData(op, eventCondition, DateTime.Now,
                    _instancesOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print($"\n\nInstances operation completed. Result = {Timers.Last().ToString()}\n\n");
                _instancesOperationTimer.Restart();
                break;
            case Realm.Element:
                _elementOperationTimer.Stop();
                Timers.Add(new TimerEventData(op, eventCondition, DateTime.Now,
                    _elementOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print($"\n\nElement operation completed. Result = {Timers.Last().ToString()}\n\n");
                _elementOperationTimer.Restart();
                break;
            case Realm.ElementType:
                _elementTypeOperationTimer.Stop();
                Timers.Add(new TimerEventData(op, eventCondition, DateTime.Now,
                    _elementTypeOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print(
                    $"\n\nElementType operation completed. Result = {Timers.Last().ToString()}\n\n");
                _elementTypeOperationTimer.Restart();
                break;
            case Realm.Group:
                _groupOperationTimer.Stop();
                Timers.Add(new TimerEventData(op, eventCondition, DateTime.Now,
                    _groupOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print($"\n\nGroup operation completed. Result = {Timers.Last().ToString()}\n\n");
                _groupOperationTimer.Restart();
                break;
            case Realm.View:
                _viewOperationTimer.Stop();
                Timers.Add(new TimerEventData(op, eventCondition, DateTime.Now,
                    _viewOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print($"\n\nView operation completed. Result = {Timers.Last().ToString()}\n\n");
                _viewOperationTimer.Restart();
                break;
            case Realm.Schedule:
                _scheduleOperationTimer.Stop();
                Timers.Add(new TimerEventData(op, eventCondition, DateTime.Now,
                    _scheduleOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print($"\n\nSchedule operation completed. Result = {Timers.Last().ToString()}\n\n");
                _scheduleOperationTimer.Restart();
                break;
            case Realm.Link:
                _linkOperationTimer.Stop();
                Timers.Add(new TimerEventData(op, eventCondition, DateTime.Now,
                    _linkOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print($"\n\nLink operation completed. Result = {Timers.Last().ToString()}\n\n");
                _linkOperationTimer.Restart();
                break;
            case Realm.Family:
                _familyOperationTimer.Stop();
                Timers.Add(new TimerEventData(op, eventCondition, DateTime.Now,
                    _familyOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print($"\n\nFamily operation completed. Result = {Timers.Last().ToString()}\n\n");
                _familyOperationTimer.Restart();
                break;
            case Realm.Category:
                _categoryOperationTimer.Stop();
                Timers.Add(new TimerEventData(op, eventCondition, DateTime.Now,
                    _categoryOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print($"\n\nCategory operation completed. Result = {Timers.Last().ToString()}\n\n");
                _categoryOperationTimer.Restart();
                break;
            case Realm.Document:
                _documentOperationTimer.Stop();
                Timers.Add(new TimerEventData(op, eventCondition, DateTime.Now,
                    _documentOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print($"\n\nDocument operation completed. Result = {Timers.Last().ToString()}\n\n");
                _documentOperationTimer.Restart();
                break;
            case Realm.RevitDatabase:
                _revitDatabaseOperationTimer.Stop();
                Timers.Add(new TimerEventData(op, eventCondition, DateTime.Now,
                    _revitDatabaseOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print(
                    $"\n\nRevitDatabase operation completed. Result = {Timers.Last().ToString()}\n\n");
                _revitDatabaseOperationTimer.Restart();
                break;
            case Realm.RevitUIApplication:
                _revitUIApplicationOperationTimer.Stop();
                Timers.Add(new TimerEventData(op, eventCondition, DateTime.Now,
                    _revitUIApplicationOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print(
                    $"\n\nRevitUIApplication operation completed. Result = {Timers.Last().ToString()}\n\n");
                _revitUIApplicationOperationTimer.Restart();
                break;
            case Realm.File:
                _fileOperationTimer.Stop();
                Timers.Add(new TimerEventData(op, eventCondition, DateTime.Now,
                    _fileOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print($"\n\nFile operation completed. Result = {Timers.Last().ToString()}\n\n");
                _fileOperationTimer.Restart();
                break;
            default:
                Debug.Print("\n\nUnknown Realm. No operation performed.\n\n");
                break;
        }
    }
}