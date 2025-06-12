using System.Diagnostics;
using System.Text.Json;
using System.Windows.Documents;
using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.RevitApi;
using Direwolf.Extensions;
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
        Direwolf.HookTimers(Application.ControlledApplication);
        Application.ControlledApplication.DocumentOpened += (sender, args) =>
        {
            _ = sender;
            Direwolf.GetDatabase(args.Document); // populate database
        };
        Application.ControlledApplication.DocumentChanged += (sender, args) =>
        {
            _ = sender;
            var doc = args.GetDocument();
            if (args.GetAddedElementIds().Count != 0)
                foreach (var element in args.GetAddedElementIds())
                {
                    Direwolf.GetDatabase(args.GetDocument())!.AddOrUpdateRevitElement(doc.GetElement(element).UniqueId, doc);
                }

            if (args.GetDeletedElementIds().Count != 0)
                foreach (var element in args.GetAddedElementIds())
                {
                    Direwolf.GetDatabase(args.GetDocument())!.DeleteRevitElement(doc.GetElement(element).UniqueId, doc);
                }

            if (args.GetModifiedElementIds().Count != 0)
                foreach (var element in args.GetModifiedElementIds())
                {
                    Direwolf.GetDatabase(args.GetDocument())!.AddOrUpdateRevitElement(doc.GetElement(element).UniqueId,
                        doc);
                }
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
    }
}