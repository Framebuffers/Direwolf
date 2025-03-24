using Autodesk.Revit.DB;

namespace Direwolf.Revit.Howls.ModelHealth
{
    public record class GetFamilyIntrospection : RevitHowl
    {
        public GetFamilyIntrospection(Document doc) => SetRevitDocument(doc);
    }
}
