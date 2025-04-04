using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls;

public record GetGridLineCount : RevitHowl
{
    public GetGridLineCount(Document doc)
    {
        SetRevitDocument(doc);
    }

    public override bool Execute()
    {
        var d = new Dictionary<string, object>
        {
            ["gridLines"] = new FilteredElementCollector(GetRevitDocument())
                .OfClass(typeof(Grid))
                .GetElementCount()
        };
        SendCatchToCallback(new Prey(d));
        return true;
    }
}