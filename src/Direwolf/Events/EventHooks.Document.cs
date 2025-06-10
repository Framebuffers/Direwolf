using System.Diagnostics;
using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.Telemetry;
using Document = Autodesk.Revit.DB.Document;

namespace Direwolf.Events;

// Unimplemented feature as of 2025-05-29
public partial class EventManager
{
    private Document? Document { get; set; }

    private Queue<Action> OnDocumentOpened { get; } = [];

    public void EnqueueOnDocumentOpened(Action action)
    {
        OnDocumentOpened.Enqueue
            (action);
    }

    public Action DequeueOnDocumentOpened()
    {
        return OnDocumentOpened.Dequeue();
    }

    public void QueueOnDocumentOpened(Action action)
    {
        if (Document is null) return;
        OnDocumentOpened.Enqueue
            (action);
    }

    private void OnDocumentClosing()
    {
        application.DocumentClosing += (sender, args) =>
        {
            Debug.Print
                ("Document is closing.");
            _documentOperationTimer.Start();
        };
        application.DocumentClosed += (sender, args) =>
        {
            AddTimeIntervalCheck
            (Method.RevitDocument,
                EventCondition.OnClosing);
        };
    }

    private void OnDocumentOpening()
    {
        application.DocumentOpening += (sender, args) =>
        {
            Debug.Print
                ("Document is opening.");
            _documentOperationTimer.Restart();
        };
        application.DocumentOpened += (sender, args) =>
        {
            Document = args.Document;

            // Run each action in the queue.
            foreach (var act in OnDocumentOpened.Select
                         (action => DequeueOnDocumentOpened()))
                act?.Invoke();

            AddTimeIntervalCheck
            (Method.RevitDocument,
                EventCondition.OnOpening);
        };
    }

    private void MeasureDocumentChangeCount()
    {
        application.DocumentChanged += (sender, args) =>
        {
            Counters.Add
            (new TriggerEventData(Method.RevitDocument,
                EventCondition.OnModifying,
                DateTime.Now));
        };
    }

    private void MeasureDocumentSavingTime()
    {
        application.DocumentSaving += (sender, args) =>
        {
            Debug.Print
                ("Document is saving.");
            _documentOperationTimer.Restart();
        };
        application.DocumentSavingAs += (sender, args) =>
        {
            Debug.Print
                ("Document is saving as.");
            _documentOperationTimer.Restart();
        };
        application.DocumentSaved += (sender, args) =>
        {
            AddTimeIntervalCheck
            (Method.RevitDocument,
                EventCondition.OnSaving);
        };
        application.DocumentSavedAs += (sender, args) =>
        {
            AddTimeIntervalCheck
            (Method.RevitDocument,
                EventCondition.OnSavingAs);
        };
    }
}