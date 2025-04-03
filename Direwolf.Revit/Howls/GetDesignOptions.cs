using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls;

public record class GetDesignOptions : RevitHowl
{
    public GetDesignOptions(Document doc)
    {
        SetRevitDocument(doc);
    }

    public override bool Execute()
    {
        using FilteredElementCollector collector = new(GetRevitDocument());
        ICollection<DesignOption> elements = collector
            .WhereElementIsNotElementType()
            .OfClass(typeof(DesignOption))
            .Cast<DesignOption>()
            .ToList();

        List<string> designOptionNames = [];
        designOptionNames.AddRange(collector.Select(element => element.Name));

        var d = new Dictionary<string, object>
        {
            ["designOptions"] = designOptionNames
        };
        SendCatchToCallback(new Prey(d));
        return true;
    }
}