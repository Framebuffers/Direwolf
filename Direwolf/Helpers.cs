using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace Direwolf;
public static class Helpers
{
    public static void GenerateNewWindow(string title, string content)
    {
        using TaskDialog t = new(title)
        {
            MainContent = content
        };
        t.Show();
    }   

    public readonly record struct RevitAppDoc(ExternalCommandData ExternalCommandData)
    {
        public static UIApplication GetApplication(ExternalCommandData cmd) => cmd.Application;
        public static Document GetDocument(ExternalCommandData cmd) => cmd.Application.ActiveUIDocument.Document;
    }
}


