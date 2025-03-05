using System;
using Autodesk.Revit.UI;

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
}


