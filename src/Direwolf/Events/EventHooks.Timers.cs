using System.Diagnostics;
using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.Telemetry;

namespace Direwolf.Events;

// Unimplemented feature as of 2025-05-29
public partial class EventManager
{
    private readonly Stopwatch _categoryOperationTimer = new();
    private readonly Stopwatch _cloudWorkshareOperationTimer = new();
    private readonly Stopwatch _dataWarehouseOperationTimer = new();
    private readonly Stopwatch _direwolfInternalOperationTimer = new();
    private readonly Stopwatch _direwolfQueryOperationTimer = new();
    private readonly Stopwatch _direwolfSetOperationTimer = new();
    private readonly Stopwatch _documentOperationTimer = new();
    private readonly Stopwatch _elementOperationTimer = new();
    private readonly Stopwatch _elementTypeOperationTimer = new();
    private readonly Stopwatch _familyOperationTimer = new();
    private readonly Stopwatch _fileOperationTimer = new();
    private readonly Stopwatch _groupOperationTimer = new();
    private readonly Stopwatch _instancesOperationTimer = new();
    private readonly Stopwatch _linkOperationTimer = new();
    private readonly Stopwatch _localWorkshareOperationTimer = new();
    private readonly Stopwatch _parameterOperationTimer = new();
    private readonly Stopwatch _revitDatabaseOperationTimer = new();
    private readonly Stopwatch _revitUIApplicationOperationTimer = new();
    private readonly Stopwatch _scheduleOperationTimer = new();
    private readonly Stopwatch _viewOperationTimer = new();
    private readonly Stopwatch _worksharingOperationTimer = new();

    private void AddTimeIntervalCheck(Realm op, EventCondition eventCondition)
    {
        switch (op)
        {
            case Realm.Query:
                _direwolfQueryOperationTimer.Stop();
                Timers.Add
                (new TimerEventData(op,
                    eventCondition,
                    DateTime.Now,
                    _direwolfQueryOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print
                    ($"DirewolfQuery operation completed. Result = {Timers.Last().ToString()}");
                _direwolfQueryOperationTimer.Restart();

                break;
            case Realm.Set:
                _direwolfSetOperationTimer.Stop();
                Timers.Add
                (new TimerEventData(op,
                    eventCondition,
                    DateTime.Now,
                    _direwolfSetOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print
                    ($"DirewolfSet operation completed. Result = {Timers.Last().ToString()}");
                _direwolfSetOperationTimer.Restart();

                break;
            case Realm.InternalDatabase:
                _direwolfInternalOperationTimer.Stop();
                Timers.Add
                (new TimerEventData(op,
                    eventCondition,
                    DateTime.Now,
                    _direwolfInternalOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print
                    ($"DirewolfInternal operation completed. Result = {Timers.Last().ToString()}");
                _direwolfInternalOperationTimer.Restart();

                break;
            case Realm.Parameter:
                _parameterOperationTimer.Stop();
                Timers.Add
                (new TimerEventData(op,
                    eventCondition,
                    DateTime.Now,
                    _parameterOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print
                    ($"Parameter operation completed. Result = {Timers.Last().ToString()}");
                _parameterOperationTimer.Restart();

                break;
            case Realm.Instances:
                _instancesOperationTimer.Stop();
                Timers.Add
                (new TimerEventData(op,
                    eventCondition,
                    DateTime.Now,
                    _instancesOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print
                    ($"Instances operation completed. Result = {Timers.Last().ToString()}");
                _instancesOperationTimer.Restart();

                break;
            case Realm.Element:
                _elementOperationTimer.Stop();
                Timers.Add
                (new TimerEventData(op,
                    eventCondition,
                    DateTime.Now,
                    _elementOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print
                    ($"Element operation completed. Result = {Timers.Last().ToString()}");
                _elementOperationTimer.Restart();

                break;
            case Realm.ElementType:
                _elementTypeOperationTimer.Stop();
                Timers.Add
                (new TimerEventData(op,
                    eventCondition,
                    DateTime.Now,
                    _elementTypeOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print
                    ($"ElementType operation completed. Result = {Timers.Last().ToString()}");
                _elementTypeOperationTimer.Restart();

                break;
            case Realm.Group:
                _groupOperationTimer.Stop();
                Timers.Add
                (new TimerEventData(op,
                    eventCondition,
                    DateTime.Now,
                    _groupOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print
                    ($"Group operation completed. Result = {Timers.Last().ToString()}");
                _groupOperationTimer.Restart();

                break;
            case Realm.View:
                _viewOperationTimer.Stop();
                Timers.Add
                (new TimerEventData(op,
                    eventCondition,
                    DateTime.Now,
                    _viewOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print
                    ($"View operation completed. Result = {Timers.Last().ToString()}");
                _viewOperationTimer.Restart();

                break;
            case Realm.Schedule:
                _scheduleOperationTimer.Stop();
                Timers.Add
                (new TimerEventData(op,
                    eventCondition,
                    DateTime.Now,
                    _scheduleOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print
                    ($"Schedule operation completed. Result = {Timers.Last().ToString()}");
                _scheduleOperationTimer.Restart();

                break;
            case Realm.Link:
                _linkOperationTimer.Stop();
                Timers.Add
                (new TimerEventData(op,
                    eventCondition,
                    DateTime.Now,
                    _linkOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print
                    ($"Link operation completed. Result = {Timers.Last().ToString()}");
                _linkOperationTimer.Restart();

                break;
            case Realm.Family:
                _familyOperationTimer.Stop();
                Timers.Add
                (new TimerEventData(op,
                    eventCondition,
                    DateTime.Now,
                    _familyOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print
                    ($"Family operation completed. Result = {Timers.Last().ToString()}");
                _familyOperationTimer.Restart();

                break;
            case Realm.Category:
                _categoryOperationTimer.Stop();
                Timers.Add
                (new TimerEventData(op,
                    eventCondition,
                    DateTime.Now,
                    _categoryOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print
                    ($"Category operation completed. Result = {Timers.Last().ToString()}");
                _categoryOperationTimer.Restart();

                break;
            case Realm.Document:
                _documentOperationTimer.Stop();
                Timers.Add
                (new TimerEventData(op,
                    eventCondition,
                    DateTime.Now,
                    _documentOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print
                    ($"Document operation completed. Result = {Timers.Last().ToString()}");
                _documentOperationTimer.Restart();

                break;
            case Realm.RevitDatabase:
                _revitDatabaseOperationTimer.Stop();
                Timers.Add
                (new TimerEventData(op,
                    eventCondition,
                    DateTime.Now,
                    _revitDatabaseOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print
                    ($"RevitDatabase operation completed. Result = {Timers.Last().ToString()}");
                _revitDatabaseOperationTimer.Restart();

                break;
            case Realm.RevitUIApplication:
                _revitUIApplicationOperationTimer.Stop();
                Timers.Add
                (new TimerEventData(op,
                    eventCondition,
                    DateTime.Now,
                    _revitUIApplicationOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print
                    ($"RevitUIApplication operation completed. Result = {Timers.Last().ToString()}");
                _revitUIApplicationOperationTimer.Restart();

                break;
            case Realm.File:
                _fileOperationTimer.Stop();
                Timers.Add
                (new TimerEventData(op,
                    eventCondition,
                    DateTime.Now,
                    _fileOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print
                    ($"File operation completed. Result = {Timers.Last().ToString()}");
                _fileOperationTimer.Restart();

                break;
            default:
                Debug.Print
                    ("Unknown Realm. No operation performed.");

                break;
        }
    }
}