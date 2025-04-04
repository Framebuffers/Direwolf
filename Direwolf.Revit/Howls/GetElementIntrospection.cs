using Autodesk.Revit.DB;

namespace Direwolf.Revit.Howls;

public record GetElementIntrospection : RevitHowl
{
    public GetElementIntrospection(Document doc)
    {
        SetRevitDocument(doc);
    }
}