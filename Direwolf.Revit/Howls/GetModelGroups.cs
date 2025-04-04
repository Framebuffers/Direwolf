using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls;

public record GetModelGroups : RevitHowl
{
    public GetModelGroups(Document doc)
    {
        SetRevitDocument(doc);
    }

    public override bool Execute()
    {
        ICollection<Element> modelGroups = new FilteredElementCollector(GetRevitDocument())
            .OfClass(typeof(Group))
            .WhereElementIsNotElementType()
            .ToList();

        var results = new Dictionary<string, string>();
        foreach (var element in modelGroups) results[element.UniqueId] = element.Id.Value.ToString();

        var d = new Dictionary<string, object>
        {
            ["modelGroups"] = results
        };
        SendCatchToCallback(new Prey(d));
        return true;
    }
}