using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;

namespace Direwolf.Revit.UI
{
    /// <summary>
    /// UI instance of Direwolf.
    /// 
    /// All commands are still routed through Direwolf.Revit.dll (and Direwolf.dll), therefore it is independent of the Revit file itself.
    /// 
    /// </summary>
    public class DirewolfUIApp : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
             RibbonPanel rp = application.CreateRibbonPanel("Database");

            PushButtonData elementInfo = new("elementToDB", "Send Element to DB", Libraries.References.AssemblyLocation, "Direwolf.Revit.UI.Commands.GetSelectedElementInfo");

            PushButtonData modelInfo = new("modelToDB", "Send Model to DB", Libraries.References.AssemblyLocation, "Direwolf.Revit.UI.Commands.GetModelInfo");

            PushButtonData aboutInfo = new("direwolfInfo", "About", Libraries.References.AssemblyLocation, "Direwolf.Revit.UI.Commands.GetInfo");
            rp.AddStackedItems(elementInfo, modelInfo, aboutInfo);
            

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
