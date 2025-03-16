using Autodesk.Revit.UI;
using System.Reflection;
using Direwolf.Revit.Benchmarking;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Direwolf.Revit.Howlers;
using Direwolf.Definitions;
using Direwolf.Revit.Howls;
using Revit.Async;
using System.Diagnostics;
using System.Text.Json;

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
            PushButtonData b = new("modelHealth", "Get Model Health", References.DirewolfRevitLocation, "Direwolf.Revit.Benchmarking.BaseCommandSanityCheck");

            PushButton pb = rp.AddItem(b) as PushButton;
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
