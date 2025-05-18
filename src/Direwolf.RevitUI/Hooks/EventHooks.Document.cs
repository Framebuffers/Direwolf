using System.Diagnostics;
using Direwolf.Dto.InternalDb.Enums;
using Direwolf.Dto.Mapper;
using Direwolf.Dto.RevitApi;
using Direwolf.Extensions;
using Direwolf.RevitUI.Extensions;

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
            AddTimeIntervalCheck(Realm.Document, EventCondition.OnClosing);
            this.Debug_PrintContentsOfCheckRegistry();
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
            AddTimeIntervalCheck(Realm.Document, EventCondition.OnOpening);
            List<RevitElement> _database = [];
            var doc = args.Document;
            if (doc is null) return;

            var result = doc.GetRevitDatabase();
            foreach (var element in result)
            {
                if (element is null) return;
                _database.Add(RevitElement.Create(doc, element));
                $"Added element: {element.Value}".ToConsole();
            }

            $"Final Count: {_database.Count}".ToConsole();
        };
    }

    private void MeasureDocumentChangeCount()
    {
        application.DocumentChanged += (sender, args) =>
        {
            Counters.Add(new TriggerEventData(Realm.Document, EventCondition.OnModifying, DateTime.Now));
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
            AddTimeIntervalCheck(Realm.Document, EventCondition.OnSaving);
        };
        application.DocumentSavedAs += (sender, args) =>
        {
            AddTimeIntervalCheck(Realm.Document, EventCondition.OnSavingAs);
        };
    }
}