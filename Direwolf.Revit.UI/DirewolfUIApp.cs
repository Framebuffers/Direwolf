using Autodesk.Revit.UI;
using Direwolf.Revit.UI.Libraries;

namespace Direwolf.Revit.UI;

/// <summary>
///     UI instance of Direwolf.
///     All commands are still routed through Direwolf.Revit.dll (and Direwolf.dll), therefore it is independent of the
///     Revit file itself.
/// </summary>
public class DirewolfUIApp : IExternalApplication
{
    public Result OnStartup(UIControlledApplication application)
    {
        var rp = application.CreateRibbonPanel("Database");

        PushButtonData elementInfo = new("elementToDB", "Send Element to DB", References.AssemblyLocation,
            "Direwolf.Revit.UI.Commands.GetSelectedElementInfo");

        PushButtonData modelInfo = new("modelToDB", "Send Model to DB", References.AssemblyLocation,
            "Direwolf.Revit.UI.Commands.GetModelInfo");

        PushButtonData aboutInfo = new("direwolfInfo", "About", References.AssemblyLocation,
            "Direwolf.Revit.UI.Commands.GetInfo");

        rp.AddStackedItems(modelInfo, elementInfo);
        rp.AddItem(aboutInfo);

        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        return Result.Succeeded;
    }
}