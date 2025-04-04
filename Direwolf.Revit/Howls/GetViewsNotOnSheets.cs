using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls;

public record GetViewsNotOnSheets : RevitHowl
{
    public GetViewsNotOnSheets(Document doc)
    {
        SetRevitDocument(doc);
    }

    public override bool Execute()
    {
        using var viewCollector = new FilteredElementCollector(GetRevitDocument())
            .OfClass(typeof(View))
            .WhereElementIsNotElementType();

        var viewportCollector = new FilteredElementCollector(GetRevitDocument())
            .OfClass(typeof(Viewport));

        HashSet<ElementId> viewsOnSheets = [.. viewportCollector.Select(vp => (vp as Viewport).ViewId)];

        List<string> viewsNotOnSheets = [];

        foreach (var viewElement in viewCollector)
            if (viewElement is View view && !view.IsTemplate && !viewsOnSheets.Contains(view.Id))
                viewsNotOnSheets.Add($"View Name: {view.Name}, View ID: {view.Id}");

        var d = new Dictionary<string, object>
        {
            ["viewsNotOnSheets"] = viewsNotOnSheets
        };
        SendCatchToCallback(new Prey(d));
        return true;
    }
}