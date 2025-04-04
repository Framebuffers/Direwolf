using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls;

public record GetViews : RevitHowl
{
    public GetViews(Document doc)
    {
        SetRevitDocument(doc);
    }

    public override bool Execute()
    {
        using var viewCollector = new FilteredElementCollector(GetRevitDocument())
            .OfClass(typeof(View))
            .WhereElementIsNotElementType();

        List<string> views = [];

        foreach (var elem in viewCollector)
            if (elem is View view && !view.IsTemplate)
                views.Add(view.Name);

        var d = new Dictionary<string, object>
        {
            ["views"] = views
        };
        SendCatchToCallback(new Prey(d));
        return true;
    }
}