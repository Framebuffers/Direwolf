using Autodesk.Revit.UI;

namespace Direwolf.Revit.UI.Libraries
{
    public static class RevitUIExtensions
    {
        public static void GenerateNewWindow(this TaskDialog t, string title, string content)
        {
            t = new(title)
            {
                MainContent = content
            };
            t.Show();
        }
    }
}
