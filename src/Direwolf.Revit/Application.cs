using System.Diagnostics;
using System.Text.Json;
using Direwolf.Definitions;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.LLM;
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
    public override void OnStartup()
    {
        CreateRibbon();
        Application.ControlledApplication.DocumentOpened += (sender, args) =>
        {
            _ = sender;
            Direwolf.GetInstance();
        };
        Application.ControlledApplication.DocumentChanged += (sender, args) =>
        {
            _ = sender;
            var doc = args.GetDocument();
            try
            {
                var addedIds = args.GetAddedElementIds();
                var modifiedIds = args.GetModifiedElementIds();
                if (addedIds.Count != 0 || modifiedIds.Count != 0)
                {
                    var combinedIds = addedIds.Concat(modifiedIds);
                    var uuids = combinedIds.Select(x => doc.GetElement(x).UniqueId).ToArray();
                    Direwolf.GetInstance().AddOrUpdateRevitElement(uuids, doc, out var _);
                }

                var deletedIds = args.GetDeletedElementIds();
                if (deletedIds.Count == 0) return;
                {
                    var uuids = deletedIds.Select(x => doc.GetElement(x).UniqueId).ToArray();
                    Direwolf.GetInstance().DeleteRevitElement(uuids, doc, out var _);
                }
            }
            finally
            {
                doc?.Dispose();
            }
        };
    }

    private void CreateRibbon()
    {
        var panel = Application.CreatePanel("Tests", "Direwolf");
        var stackPanel = panel.AddStackPanel();
        stackPanel.AddPushButton<ExportCacheToJson>("From Cache");
        stackPanel.AddPushButton<ExportToJsonFromScratch>("From Disk");
        stackPanel.AddLabel("LLM");
        panel.AddPushButton<TestCommands>("Run Tests");
        panel.AddPushButton<About>("About").SetImage("Resources/Icons/RibbonIcon16.png");
        panel.AddPushButton<CheckStabilityOfElements>("Check ElementIDs");
        panel.AddPushButton<Prompt>("Prompt");
        var x = panel.AddTextBox("prompt");
        x.PromptText = "Prompt Data";
        //TODO: fix with new schema
        
        // x.EnterPressed += (sender, args) =>
        // {
        //     var howl = Howl.Create(DataType.String, RequestType.Get, new Dictionary<string, object>()
        //     {
        //         ["data"] = x.Value
        //     }, "", "entry");
        //
        //     var wolfpack = Wolfpack.Create(RequestType.Get, "PromptFromUI",
        //         WolfpackParams.Create(howl, $"wolfpack://com.revit.autodesk-2025/direwolf/custom?t=PromptFromUI"),
        //         [McpResourceContainer.Create(howl)], null);
        //
        //     var asPrompt = Wolfpack.AsPrompt(wolfpack, "getDataFromRevitUI", "Gets data from the Revit UI",
        //         $"wolfpack://com.revit.autodesk-2025/direwolf/custom?t=PromptFromUI");
        //     
        //     var serialized = JsonSerializer.Serialize(asPrompt, new JsonSerializerOptions(){WriteIndented = true});
        //     Debug.Print(serialized);
        // };
    }
}