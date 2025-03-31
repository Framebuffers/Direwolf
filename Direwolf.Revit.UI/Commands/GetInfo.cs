using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System.Diagnostics;
using Direwolf.Revit.Howlers;
using Revit.Async;
using Direwolf.Definitions;
using Direwolf.Revit.Howls;

namespace Direwolf.Revit.UI.Commands
{

    /// <summary>
    /// Benchmark code.
    /// </summary>
    [Transaction(TransactionMode.Manual)]

    public partial class GetInfo : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            string githubLink = @"<a href=""https://github.com/Framebuffers/Direwolf"" title=""information icons"">Link to GitHub repository.</a>";

            string aboutIcon = @"<a href=""https://www.flaticon.com/free-icons/information"" title=""information icons"">Information icons created by Anggara - Flaticon</a>";

            string dbIcon = @"<a href=""https://www.flaticon.com/free-icons/database"" title=""database icons"">Database icons created by srip - Flaticon</a>";

            string elementIcon = @"<a href=""https://www.flaticon.com/free-icons/augmented-reality"" title=""augmented reality icons"">Augmented reality icons created by juicy_fish - Flaticon</a>";

            try
            {
                TaskDialog d = new("About Direwolf")
                {
                    MainInstruction = "Direwolf for Revit",
                    MainContent = $"Data Analysis Framework for Autodesk Revit. This is a proof of concept. Please check the repository link below for more information.",
                    FooterText = $"{aboutIcon}\n{dbIcon}\n{elementIcon}\n\n{githubLink}"
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
}
