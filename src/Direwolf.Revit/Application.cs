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
                    Direwolf.GetInstance().AddOrUpdateRevitElement(uuids, doc);
                }

                var deletedIds = args.GetDeletedElementIds();
                if (deletedIds.Count == 0) return;
                {
                    var uuids = deletedIds.Select(x => doc.GetElement(x).UniqueId).ToArray();
                    Direwolf.GetInstance().DeleteRevitElement(uuids, doc);
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
        var panel = Application.CreatePanel("Export", "Direwolf");
        panel.AddPushButton<ExportCacheToJson>("From Cache")
            .SetImage("/Direwolf.Revit;component/Resources/Icons/Placeholder/slice1_16.png")
            .SetLargeImage("/Direwolf.Revit;component/Resources/Icons/Placeholder/slice_32.png");
        panel.AddPushButton<ExportToJsonFromScratch>("From Disk")
            .SetImage("/Direwolf.Revit;component/Resources/Icons/Placeholder/slice1_16.png")
            .SetLargeImage("/Direwolf.Revit;component/Resources/Icons/Placeholder/slice_32.png");
        panel.AddPushButton<About>("About")
            .SetImage("/Direwolf.Revit;component/Resources/Icons/Placeholder/slice1_16.png")
            .SetLargeImage("/Direwolf.Revit;component/Resources/Icons/Placeholder/slice_32.png");
        var ai = Application.CreatePanel("MCP", "Direwolf");
        ai.AddPushButton<CheckStabilityOfElements>("Check IDs")
            .SetImage("/Direwolf.Revit;component/Resources/Icons/Placeholder/slice1_16.png)")
            .SetLargeImage("/Direwolf.Revit;component/Resources/Icons/Placeholder/slice_32.png");
        ai.AddPushButton<AnthopicClient>("Init MCP Client")
            .SetImage("/Direwolf.Revit;component/Resources/Icons/Placeholder/slice1_16.png")
            .SetLargeImage("/Direwolf.Revit;component/Resources/Icons/Placeholder/slice_32.png");
        ai.AddPushButton<TestCommands>("Self Test")
            .SetImage("/Direwolf.Revit;component/Resources/Icons/Placeholder/slice1_16.png")
            .SetLargeImage("/Direwolf.Revit;component/Resources/Icons/Placeholder/slice_32.png");
    }
}