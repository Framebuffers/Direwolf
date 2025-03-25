using Autodesk.Revit.DB;

namespace Direwolf.Revit.Howls
{
    public record class GetFamilyIntrospection : RevitHowl
    {
        public GetFamilyIntrospection(Document doc) => SetRevitDocument(doc);
    }
}
