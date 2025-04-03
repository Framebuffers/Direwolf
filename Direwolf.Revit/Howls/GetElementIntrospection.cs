using Autodesk.Revit.DB;

namespace Direwolf.Revit.Howls;

public record class GetElementIntrospection : RevitHowl
{
    public GetElementIntrospection(Document doc)
    {
        SetRevitDocument(doc);
    }
}