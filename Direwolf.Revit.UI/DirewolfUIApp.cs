using Autodesk.Revit.UI;

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
            Buttons.AddTrackThisElementButton(rp);
            Buttons.AddTrackThisDocumentButton(rp);
            Buttons.AddDocumentSnapshotButton(rp);
            Buttons.AddDocumentToJsonButton(rp);
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
