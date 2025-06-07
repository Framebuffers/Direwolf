using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace Direwolf.Revit.Commands;

/// <summary>
///     External command entry point
/// </summary>
[UsedImplicitly]
[Transaction
    (TransactionMode.Manual)]
public class About : ExternalCommand
{
    public override void Execute()
    {
        var t = new TaskDialog
            ("About Direwolf")
            {
                MainInstruction = "About Direwolf",
                MainContent = $"Direwolf v0.4-alpha."
                              + $"Payload Hunting Framework for Autodesk Revit."
                              + $""
                              + $"Copyright (C) 2025 Sebastian Torres Sagredo"
                              + $""
                              + $"Licenced under the Apache-2.0 licence."
                              + $"",
                FooterText = "<a href=\"https://github.com/framebuffers/direwolf \">"
                             + "Check out the GitHub repo here</a>"
            };
        t.Show();
    }
}