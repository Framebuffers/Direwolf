using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Direwolf.Revit.Definitions;
using Direwolf.Revit.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Direwolf.Revit.Extensions
{
    public static class DocumentExtensions
    {
        public static FilteredElementCollector GetElementsByType(this Document doc, ElementClassFilter f)
        {
            return new FilteredElementCollector(doc).WherePasses(f);
        }

        public static DocumentIntrospection GetDocumentIntrospection(this Document doc) => new(doc);
        public static ProjectInformationIntrospection GetProjectInformationIntrospection(this Document doc) => new(doc);
        public static ProjectSiteIntrospection GetProjectSiteIntrospection(this Document doc) => new(doc);
        public static UnitIntrospection GetProjectUnitsIntrospection(this Document doc) => new(doc);
    }

}





