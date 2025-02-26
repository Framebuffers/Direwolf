using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Microsoft.XmlSerializer.Generator;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

namespace Direwolf.ManifestGenerator
{

    /// <summary>
    /// Attribute to automatically generate all neccesary data to create a valid Autodesk Revit .addin XML manifest. Adding these will ensure your add-in will be loaded by Revit successfully.
    /// </summary>       
    /// <param name="revitAddinInstallationPath">
    /// Fill the <b>absolute path</b> for your Addins folder. Unless it's been specified otherwise when Revit was installed, it should be:
    /// <code>C:\ProgramData\Autodesk\Revit\Addins\20xx\</code> More official documentation is available at <see href="https://www.autodesk.com/support/technical/article/caas/tsarticles/ts/7JiiE4UoWHaTjxzuF754mL.html"/>.
    /// </param>
    /// <param name="vendorName">
    /// Name of the company (or developer) creating this add-in.
    /// Defaults to "unknown"
    /// </param>
    /// <param name="addinName">
    /// The name upon which your add-in will be called inside Revit.
    /// </param>       
    /// <param name="addinDescription">
    /// A string that appears when a user mouses over your add-in's button.
    /// </param>
    /// <param name="revitDiscipline">
    /// Add-ins can be separated by each Revit Discipline. Don't know how relevant it is on Revit 2025+
    /// </param>
    /// <param name="locale">
    /// The language used by your add-in. Should be upgraded to use native .NET i18n facilities.
    /// </param>
    [AttributeUsage(AttributeTargets.Class)]
    public class ManifestAttributes(string revitAddinInstallationPath,
                              string vendorName = "unknown",
                              string vendorDescription = "unknown",
                              string addinName = "unknown",
                              string addinDescription = "unknown",
                              string revitDiscipline = "unknown",
                              string locale = "unknown",
                              bool alwaysVisible = true) : System.Attribute
    {
        private Guid ExtensionGuid => new();
    }
}