using System.Diagnostics;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

using Direwolf.Dto.InternalDb.Enums;
using Direwolf.Dto.Mapper;
using Direwolf.Dto.RevitApi;
using Direwolf.Extensions;
using Direwolf.RevitUI.Extensions;

using Document = Autodesk.Revit.DB.Document;

namespace Direwolf.RevitUI.Hooks;

public partial class EventHooks
{
    public  Document?     Document         {get; set;}
    private Queue<Action> OnDocumentOpened {get; set;} = [];

    public void EnqueueOnDocumentOpened(Action action)
    {
        OnDocumentOpened.Enqueue(action);
    }

    public Action DequeueOnDocumentOpened()
    {
        return OnDocumentOpened.Dequeue();
    }

    public void QueueOnDocumentOpened(Action action)
    {
        if (Document is null) return;
        OnDocumentOpened.Enqueue(action);
    }

    private void OnDocumentClosing()
    {
        _application.DocumentClosing += (sender, args) =>
        {
            Debug.Print("\n\nDocument is closing.\n\n");
            _documentOperationTimer.Start();
        };
        _application.DocumentClosed += (sender, args) =>
        {
            AddTimeIntervalCheck(Realm.Document,
                                 EventCondition.OnClosing);
            this.Debug_PrintContentsOfCheckRegistry();
        };
    }

    private void OnDocumentOpening()
    {
        _application.DocumentOpening += (sender, args) =>
        {
            Debug.Print("\n\nDocument is opening.\n\n");
            _documentOperationTimer.Restart();
        };
        _application.DocumentOpened += (sender, args) =>
        {
            Document = args.Document;
            // Run each action in the queue.
            foreach (var act in OnDocumentOpened.Select(action => DequeueOnDocumentOpened()))
                act?.Invoke();

            AddTimeIntervalCheck(Realm.Document,
                                 EventCondition.OnOpening);
        };
    }

    private void MeasureDocumentChangeCount()
    {
        _application.DocumentChanged += (sender, args) =>
        {
            Counters.Add(new TriggerEventData(Realm.Document,
                                              EventCondition.OnModifying,
                                              DateTime.Now));
        };
    }

    private void MeasureDocumentSavingTime()
    {
        _application.DocumentSaving += (sender, args) =>
        {
            Debug.Print("\n\nDocument is saving.\n\n");
            _documentOperationTimer.Restart();
        };
        _application.DocumentSavingAs += (sender, args) =>
        {
            Debug.Print("\n\nDocument is saving as.\n\n");
            _documentOperationTimer.Restart();
        };
        _application.DocumentSaved += (sender, args) =>
        {
            AddTimeIntervalCheck(Realm.Document,
                                 EventCondition.OnSaving);
        };
        _application.DocumentSavedAs += (sender, args) =>
        {
            AddTimeIntervalCheck(Realm.Document,
                                 EventCondition.OnSavingAs);
        };
    }
}