using System.Diagnostics;
using System.Text.Json;
using Direwolf.Definitions;
using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.Parsers;
using Direwolf.Revit.Commands;
using Direwolf.Revit.Commands.Testing;
using Nice3point.Revit.Toolkit.External;

namespace Direwolf.Revit;

/// <summary>
///     Application entry point
/// </summary>
[UsedImplicitly]
public class Application : ExternalApplication
{
    private static Document? Current;
    public override void OnStartup()
    {
        CreateRibbon();
        Direwolf.HookTimers(Application.ControlledApplication);
        Application.ControlledApplication.DocumentOpened += (sender, args) =>
        {
            _ = sender;
            Direwolf.GetDatabase(args.Document); // populate database
            Current = args.Document;
        };
        Application.ControlledApplication.DocumentChanged += (sender, args) =>
        {
            _ = sender;
            Current = args.GetDocument();
            var doc = args.GetDocument();
            if (args.GetAddedElementIds().Count != 0)
                foreach (var element in args.GetAddedElementIds())
                    Direwolf.GetDatabase(args.GetDocument())!.AddOrUpdateRevitElement(doc.GetElement(element).UniqueId,
                        doc);

            if (args.GetDeletedElementIds().Count != 0)
                foreach (var element in args.GetAddedElementIds())
                    Direwolf.GetDatabase(args.GetDocument())!.DeleteRevitElement(doc.GetElement(element).UniqueId, doc);

            if (args.GetModifiedElementIds().Count != 0)
                foreach (var element in args.GetModifiedElementIds())
                    Direwolf.GetDatabase(args.GetDocument())!.AddOrUpdateRevitElement(doc.GetElement(element).UniqueId,
                        doc);
        };
    }

    private void CreateRibbon()
    {
        var panel = Application.CreatePanel("Tests", "Direwolf");
        var stackPanel = panel.AddStackPanel();
        stackPanel.AddPushButton<ExportCacheToJson>("From Cache");
        stackPanel.AddPushButton<ExportToJsonFromScratch>("From Disk");
        stackPanel.AddLabel("Export");
        panel.AddPushButton<TestCommands>("Run Tests");
        panel.AddPushButton<About>("About").SetImage("Resources/Icons/RibbonIcon16.png");
        panel.AddPushButton<CheckStabilityOfElements>("Check ElementIDs");
        panel.AddPushButton<Prompt>("Prompt");
        var x = panel.AddTextBox("prompt");
        x.PromptText = "Prompt Data";
        x.EnterPressed += (sender, args) =>
        {
            Document doc = Revit.Application.Current;
            var howl = Howl.Create(DataType.String, RequestType.Get, new Dictionary<string, object>()
            {
                ["data"] = x.Value
            }, "", "entry");

            var wolfpack = Direwolf.CreatePrompt(doc!, "fromUI", $"taken from {doc.Title} at {DateTime.UtcNow}", howl,
                $"wolfpack://com.revit.autodesk-2025/direwolf/custom?t=fromUI");
            
            var serialized = JsonSerializer.Serialize(wolfpack, new JsonSerializerOptions(){WriteIndented = true});
            Debug.Print(serialized);
        };
    }
}