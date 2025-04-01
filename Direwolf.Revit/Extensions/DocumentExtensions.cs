using Autodesk.Revit.DB;
using Direwolf.Revit.Definitions;

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





