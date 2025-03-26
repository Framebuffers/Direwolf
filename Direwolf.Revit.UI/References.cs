using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Direwolf.Revit.Howlers;
using System.Reflection;

namespace Direwolf.Revit.UI
{
    public static class References
    {
        public static readonly string AssemblyLocation = Assembly.GetExecutingAssembly().Location;
        public static readonly string DirewolfRevitLocation = typeof(RevitHowler).Assembly.Location;
        public static Document GetRevitDocument(this ExternalCommandData cmd) => cmd.Application.ActiveUIDocument.Document;
        public static UIApplication GetUIApplication(this ExternalCommandData cmd) => cmd.Application;
    }
}
