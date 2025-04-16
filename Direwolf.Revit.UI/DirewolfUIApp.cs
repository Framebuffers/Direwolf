using Autodesk.Revit.UI;
using Direwolf.Revit.UI.Libraries;

namespace Direwolf.Revit.UI;

/// <summary>
///     UI instance of _Direwolf.
///     All commands are still routed through _Direwolf.Revit.dll (and _Direwolf.dll), therefore it is independent of the
///     Revit file itself.
/// </summary>
public class DirewolfUIApp : IExternalApplication
{
    public Result OnStartup(UIControlledApplication application)
    {
        var rp = application.CreateRibbonPanel("Database");

        // PushButtonData native = new("revitNative", "Revit Native Query", References.AssemblyLocation,
        //     "Direwolf.Revit.UI.Commands.TestingRig.NativeRevitCommand");
        PushButtonData async = new("direwolfAsync", "Revit Async Query", References.AssemblyLocation,
            "Direwolf.Revit.UI.Commands.TestingRig.RunAllTestsCommand");
        //
        // PushButtonData runAll = new("test", "Test all", References.AssemblyLocation,
        //     "Direwolf.Revit.UI.Commands.TestingRig.RunAllTestsCommand");
        rp.AddItem(async);
        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        return Result.Succeeded;
    }
}

/*
public class DirewolfTelemetryService : IExternalApplication
{
    private int ApplicationLoadTime { get; set; }
    private int DocumentLoadTime { get; set; }
    private int ResponseTimeAverage { get; set; }
    private int ViewChangeTimeAverage { get; set; }
    private int DocumentPrintingTimeAverage { get; set; }
    private int DocumentSyncTimeAverage { get; set; }
    private int DocumentSavingTimeAverage { get; set; }
    private int ElementDuplicationTimeAverage { get; set; }
    private int FailureProcessingEvents { get; set; }
    private int FileExportTimeAverage { get; set; }
    private int FileImportTimeAverage { get; set; }
    private int LinkedResourceLoadTimeAverage { get; set; }
    private int ProgressBarTimeAverage { get; set; }
    private int ViewExportTimeAverage { get; set; }
    private int ViewPrintTimeAverage { get; set; }
    private double FileSize { get; set; }
    private int FamilyCount { get; set; }
    private int WarningCount { get; set; }
    private int ViewsInsideDocument { get; set; }
    private int ViewsNotInSheets { get; set; }
    private int AnnotativeElementCount { get; set; }
    private int ExternalReferenceCount { get; set; }
    private int ModelGroupCount { get; set; }
    private int DetailGroupCount { get; set; }
    private int DesignGroupCount { get; set; }
    private int LevelCount { get; set; }
    private int GridCount { get; set; }
    private int UnenclosedRoomCount { get; set; }
    private int ViewportCount { get; set; }
    private int UnconnectedDuctCount { get; set; }
    private int UnconnectedPipeCount { get; set; }
    private int UnconnectedElectricalCount { get; set; }
    private int NonNativeStyleCount { get; set; }
    private int FlippedElementCount { get; set; }
    private int WorksetCount { get; set; }

    public Result OnStartup(UIControlledApplication application)
    {
        throw new NotImplementedException();
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        throw new NotImplementedException();
    }
}
*/