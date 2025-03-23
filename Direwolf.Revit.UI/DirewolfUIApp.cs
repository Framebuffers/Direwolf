using Autodesk.Revit.UI;
using Direwolf.Extensions;
using System.Diagnostics;

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
        public Stopwatch docLoadTime { get; set; } = new();
        public Result OnStartup(UIControlledApplication application)
        {
            RibbonPanel rp = application.CreateRibbonPanel("Direwolf");
            Buttons.AddDocumentIntrospectionButton(rp);
            application.ControlledApplication.DocumentOpening += ControlledApplication_DocumentOpening;
            application.ControlledApplication.DocumentOpened += ControlledApplication_DocumentOpened;
            return Result.Succeeded;
        }

        private void ControlledApplication_DocumentOpening(object? sender, Autodesk.Revit.DB.Events.DocumentOpeningEventArgs e)
        {
            docLoadTime.Start();
        }

        private void ControlledApplication_DocumentOpened(object? sender, Autodesk.Revit.DB.Events.DocumentOpenedEventArgs e)
        {
            docLoadTime.Stop();
            $"Time taken: {docLoadTime.Elapsed.TotalSeconds}".ToConsole();
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
