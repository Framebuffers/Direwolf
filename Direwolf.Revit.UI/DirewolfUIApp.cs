using Autodesk.Revit.UI;
using System.Reflection;
using Direwolf.Revit.Howlers;

namespace Direwolf.Revit.UI
{
    public static class References
    {
        public static readonly string AssemblyLocation = Assembly.GetExecutingAssembly().Location;
        public static readonly string DirewolfRevitLocation = typeof(RevitHowler).Assembly.Location;
    }

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
            Buttons.AddGetModelHealthButton(rp);
            //Buttons.AddPushToDBButton(rp);
            
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
