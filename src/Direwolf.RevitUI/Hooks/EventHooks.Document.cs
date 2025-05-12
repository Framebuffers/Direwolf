using System.Diagnostics;

namespace Direwolf.RevitUI.Hooks;

public partial class EventHooks
{
    private void MeasureSessionTime()
    {
        application.DocumentClosing += (sender, args) =>
        {
            Debug.Print("\n\nDocument is closing.\n\n");
            _documentOperationTimer.Start();
        };
        application.DocumentClosed += (sender, args) =>
        {
            AddTimeIntervalCheck(OperationType.DocumentOperation, ConditionType.OnClosing);     
            Debug_PrintContentsOfCheckRegistry();
        };
        
    }

    private void MeasureDocumentOpeningTime()
    {
        application.DocumentOpening += (sender, args) =>
        {
            Debug.Print("\n\nDocument is opening.\n\n");
            _documentOperationTimer.Restart();
        };
        application.DocumentOpened += (sender, args) =>
        {
            AddTimeIntervalCheck(OperationType.DocumentOperation, ConditionType.OnOpening); 
        };
    }

    private void MeasureDocumentChangeCount()
    {
        application.DocumentChanged += (sender, args) =>
        {
            _eventChecks.Add(new EventTriggerCheck(
                OperationType.DocumentOperation, 
                ConditionType.OnModifying, 
                DateTime.Now));
        };
    }

    private void MeasureDocumentSavingTime()
    {
        application.DocumentSaving += (sender, args) =>
        {
            Debug.Print("\n\nDocument is saving.\n\n");
            _documentOperationTimer.Restart();
        };

        application.DocumentSavingAs += (sender, args) =>
        {
            Debug.Print("\n\nDocument is saving as.\n\n");
            _documentOperationTimer.Restart();
        };

        application.DocumentSaved += (sender, args) =>
        {
            AddTimeIntervalCheck(OperationType.DocumentOperation, ConditionType.OnSaving);
        };

        application.DocumentSavedAs += (sender, args) =>
        {
            AddTimeIntervalCheck(OperationType.DocumentOperation, ConditionType.OnSavingAs);
        };
    }
}