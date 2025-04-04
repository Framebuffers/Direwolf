using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls;

public record GetElementsByWorkset : RevitHowl
{
    public GetElementsByWorkset(Document doc)
    {
        SetRevitDocument(doc);
    }

    public override bool Execute()
    {
        using var collector = new FilteredElementCollector(GetRevitDocument())
            .WhereElementIsNotElementType();

        Dictionary<string, int> worksetElementCount = [];

        foreach (var element in collector)
        {
            using var worksetParam = element.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);

            if (worksetParam != null)
            {
                var worksetName = worksetParam.AsValueString();

                if (worksetElementCount.TryGetValue(worksetName, out var value))
                    worksetElementCount[worksetName] = value++;
                else
                    worksetElementCount[worksetName] = 1;
            }
        }

        var d = new Dictionary<string, object>
        {
            ["elementsByWorkset"] = worksetElementCount
        };
        SendCatchToCallback(new Prey(d));
        return true;
    }
}