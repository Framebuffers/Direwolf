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