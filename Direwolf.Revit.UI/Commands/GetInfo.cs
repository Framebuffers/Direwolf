using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Direwolf.Revit.UI.Commands;

/// <summary>
///     About button.
/// </summary>
[Transaction(TransactionMode.Manual)]
public class GetInfo : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var githubLink =
            @"<a href=""https://github.com/Framebuffers/_Direwolf"" title=""information icons"">Link to GitHub repository.</a>";

        try
        {
            TaskDialog d = new("About _Direwolf")
            {
                MainInstruction = "_Direwolf for Revit",
                MainContent =
                    "Data Analysis Framework for Autodesk Revit. This is a proof of concept. Please check the repository link below for more information.",
                FooterText = githubLink
            };
            d.Show();
        }
        catch
        {
            return Result.Failed;
        }

        return Result.Succeeded;
    }
}