using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls;

public record class GetDetailGroups : RevitHowl
{
    public GetDetailGroups(Document doc)
    {
        SetRevitDocument(doc);
    }

    public override bool Execute()
    {
        using FilteredElementCollector collector = new(GetRevitDocument());
        ICollection<Group> detailGroups = collector
            .WhereElementIsNotElementType()
            .OfClass(typeof(Group))
            .Cast<Group>()
            .Where(e => e.Category != null && e.Category.Name == "Detail Groups")
            .ToList();

        var results = new Dictionary<string, string>();
        foreach (Element element in detailGroups) results[element.UniqueId] = element.Id.ToString();

        var d = new Dictionary<string, object>
        {
            ["detailGroups"] = results
        };
        SendCatchToCallback(new Prey(d));
        return true;
    }
}