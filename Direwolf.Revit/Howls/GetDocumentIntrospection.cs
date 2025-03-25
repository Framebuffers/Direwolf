using Autodesk.Revit.DB;

namespace Direwolf.Revit.Howls
{
    public record class GetDocumentIntrospection : RevitHowl
    {
        public GetDocumentIntrospection(Document doc) => SetRevitDocument(doc);
    }
}
