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
            RibbonPanel rp = application.CreateRibbonPanel("Direwolf");

            PushButtonData modelInfo = new("modelToDB", "Send Model to DB", Libraries.References.AssemblyLocation, "Direwolf.Revit.UI.Commands.GetModelInfo")
            {
                Image = new BitmapImage(new Uri("/model.png", UriKind.Relative))
            };

            PushButtonData elementInfo = new("elementToDB", "Send Element to DB", Libraries.References.AssemblyLocation, "Direwolf.Revit.UI.Commands.GetSelectedElementInfo")
            {
                Image = new BitmapImage(new Uri("/element.png", UriKind.Relative))
            };

            PushButtonData aboutInfo = new("direwolfInfo", "About", Libraries.References.AssemblyLocation, "Direwolf.Revit.UI.Commands.GetInfo")
            {
                Image = new BitmapImage(new Uri("/about.png", UriKind.Relative))
            };
            rp.AddStackedItems(modelInfo, elementInfo, aboutInfo);
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
