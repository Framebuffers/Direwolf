using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Direwolf.Revit.Utilities
{
    internal static class RevitExtensions
    {
        public static Document GetDocument(this ExternalCommandData cmd) => cmd.Application.ActiveUIDocument.Document;
    }
}