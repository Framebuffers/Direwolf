using System.Diagnostics;

using Autodesk.Revit.UI;

using Direwolf.Database;

namespace Direwolf;

public class Frontend : Nice3point.Revit.Toolkit.External.ExternalApplication
{
    public override void OnStartup()
    {
        var e = Engine.GetEngine();
        Debug.Print("\n\n\n\nOnStartup\n\n\n\n");

    }
}