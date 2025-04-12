using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions;

public sealed class ElementBTree(Document Document)
{
    public void BuildTree()
    {
        // get all parameters
        // Element e = Document.GetElement(e).EvaluateAllParameterValues();

        // get all the members of your own type
        // Element e = e.GetValidTypes();

        // gets all the members of this type
        // ElementType e = Document.GetElement(e.GetTypeId()).GetValidTypes();
    }
}