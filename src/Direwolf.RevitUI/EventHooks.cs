using System.Diagnostics;
using System.Text.Json;
using Autodesk.Revit.ApplicationServices;

namespace Direwolf.RevitUI;

internal readonly record struct TimeIntervalCheck(
    OperationType Operation,
    ConditionType Condition,
    DateTime CreatedAt,
    double IntervalMilliseconds
    );

internal readonly record struct EventTriggerCheck(
    OperationType Operation,
    ConditionType Condition,
    DateTime CreatedAt,
    bool TriggerResult = true
    );

internal enum OperationType
{
    ElementOperation,
    FamilyOperation,
    ViewOperation,
    LinkOperation,
    DocumentOperation,
    FileOperation,
    WorksharingOperation,
    FailureOperation,
    ApplicationOperation
}

internal enum ConditionType
{
    OnProgress,
    OnExecution,
    OnOpening,
    OnClosing,
    OnModifying,
    OnDeleting,
    OnSaving,
    OnSavingAs,
    OnExporting,
    OnCreating,
    OnSync,
    OnReload
}

public partial class EventHooks(ControlledApplication application)
{
    private readonly List<TimeIntervalCheck> _timeIntervalChecks = [];
    private readonly List<EventTriggerCheck> _eventChecks = [];
    private readonly Stopwatch _elementOperationTimer = new();
    private readonly Stopwatch _familyOperationTimer = new();
    private readonly Stopwatch _viewOperationTimer = new();
    private readonly Stopwatch _linkOperationTimer = new();
    private readonly Stopwatch _documentOperationTimer = new();
    private readonly Stopwatch _fileOperationTimer = new();
    private readonly Stopwatch _worksharingOperationTimer = new();
    private readonly Stopwatch _applicationOperationTimer = new();

    private void AddTimeIntervalCheck(OperationType op, ConditionType condition)
    {
        switch (op)
        {
            case OperationType.ElementOperation:
                _elementOperationTimer.Stop();
                 _timeIntervalChecks.Add(new TimeIntervalCheck(
                       op,
                       condition,
                       DateTime.Now, 
                       _elementOperationTimer.Elapsed.TotalMilliseconds));     
                Debug.Print($"\n\nElement operation completed. Result = {_timeIntervalChecks.Last().ToString()}\n\n");
                _elementOperationTimer.Restart();
                break;
            
            case OperationType.FamilyOperation:
                _familyOperationTimer.Stop();
                _timeIntervalChecks.Add(new TimeIntervalCheck(
                    op,
                    condition,
                    DateTime.Now,
                    _familyOperationTimer.Elapsed.TotalMilliseconds
                    ));
                Debug.Print($"\n\nFamily operation completed. Result = {_timeIntervalChecks.Last().ToString()}\n\n");
                _familyOperationTimer.Restart();
                break;
            
            case OperationType.ViewOperation:
                _viewOperationTimer.Stop();
                _timeIntervalChecks.Add(new TimeIntervalCheck(
                    op,
                    condition,
                    DateTime.Now,
                    _viewOperationTimer.Elapsed.TotalMilliseconds
                    ));
                Debug.Print($"\n\nView operation completed. Result = {_timeIntervalChecks.Last().ToString()}\n\n");
                _viewOperationTimer.Restart();
                break;
            
            case OperationType.LinkOperation:
                _linkOperationTimer.Stop();
                _timeIntervalChecks.Add(new TimeIntervalCheck(
                    op,
                    condition,
                    DateTime.Now, 
                    _linkOperationTimer.Elapsed.TotalMilliseconds
                    ));
                Debug.Print($"\n\nLink operation completed. Result = {_timeIntervalChecks.Last().ToString()}\n\n");
                _linkOperationTimer.Restart();
                break;
            
            case OperationType.DocumentOperation:
                _documentOperationTimer.Stop();
                _timeIntervalChecks.Add(new TimeIntervalCheck(
                    op,
                    condition,
                    DateTime.Now, 
                    _documentOperationTimer.Elapsed.TotalMilliseconds));
                Debug.Print($"\n\nDocument operation completed. Result = {_timeIntervalChecks.Last().ToString()}\n\n");
                _documentOperationTimer.Restart();
                break;
            
            case OperationType.FileOperation:
                _fileOperationTimer.Stop();
                _timeIntervalChecks.Add(new TimeIntervalCheck(
                    op,
                    condition,
                    DateTime.Now,
                    _fileOperationTimer.Elapsed.TotalMilliseconds));
                 Debug.Print($"\n\nFile operation completed. Result = {_timeIntervalChecks.Last().ToString()}\n\n");
                
                _fileOperationTimer.Restart();
                break;
            
            case OperationType.WorksharingOperation:
                _worksharingOperationTimer.Stop();
                _timeIntervalChecks.Add(new TimeIntervalCheck(
                    op,
                    condition,
                    DateTime.Now, 
                    _worksharingOperationTimer.Elapsed.TotalMilliseconds));
                 Debug.Print($"\n\nWorksharing operation completed. Result = {_timeIntervalChecks.Last().ToString()}\n\n");
                _worksharingOperationTimer.Restart();
                break;
            
            case OperationType.ApplicationOperation:
                _applicationOperationTimer.Stop();
                _timeIntervalChecks.Add(new TimeIntervalCheck(
                    op,
                    condition,
                    DateTime.Now,
                    _applicationOperationTimer.Elapsed.TotalMilliseconds));
                 Debug.Print($"\n\nApplication operation completed. Result = {_timeIntervalChecks.Last().ToString()}\n\n");
                _applicationOperationTimer.Restart();
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(op), op, null);
        }
    }

    private void Debug_PrintContentsOfCheckRegistry()
    {
        using StringWriter sw = new();
        SortedList<DateTime, TimeIntervalCheck> sortedList = [];
        foreach (var e in _timeIntervalChecks)
        {
            sortedList.Add(e.CreatedAt, e);
        }
        
        Debug.Print(JsonSerializer.Serialize(sortedList));
        //
        // foreach (var x in sortedList)
        // {
        //     Debug.Print($"{x.Key}: {x.Value.ToString()}");
        // }
    }
    
    // Categories of Events inside ControlledApplication
    // Revit 2025
    //
    // DocumentOperations
    //      OnClosing
    //          DocumentClosing
    //          DocumentClosed
    //      OnOpening
    //          DocumentOpening
    //          DocumentOpened
    //      OnModifying
    //          DocumentChanged
    //      OnSaving
    //          DocumentSaving
    //          DocumentSaved
    //          DocumentSavingAs
    //          DocumentSavedAs
    //      OnExporting
    //          DocumentPrinting
    //          DocumentPrinted
    //      OnCreating
    //          DocumentCreating
    //          DocumentCreated
    // WorksharingOperations
    //      OnSync
    //          DocumentSynchronizingWithCentral
    //          DocumentSynchronizedWithCentral
    //      OnReload
    //          DocumentReloadingLatest
    //          DocumentReloadedLatest
    //      OnProgress
    //          WorksharedOperationProgressChanged
    // ElementOperations
    //      ElementTypeDuplication
    //      ElementTypeDuplicated
    // ViewOperations
    //      ViewPrinting
    //      ViewPrinted
    // FileOperations
    //      OnImport
    //          FileImporting
    //          FileImported
    //      OnExport
    //          FileExporting
    //          FileExported
    // FamilyOperations
    //      FamilyLoadingIntoDocument
    //      FamilyLoadedIntoDocument
    // FailureOperations
    //      FailuresProcessing
    // LinkOperations
    //      LinkedResourceOpening
    //      LinkedResourceOpened
    // ApplicationOperations
    //      ApplicationInitialized
    
    public void LoadTimers()
    {
        // Document Hooks
        MeasureDocumentOpeningTime();
        MeasureSessionTime();
        MeasureDocumentChangeCount();
        MeasureDocumentSavingTime();
    }
   
}