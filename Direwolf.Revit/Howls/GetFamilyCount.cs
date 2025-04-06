using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls;

public record GetFamilyCount : RevitHowl
{
    public GetFamilyCount(Document doc)
    {
        SetRevitDocument(doc);
    }

    public override bool Hunt()
    {
        using FilteredElementCollector familyCollector = new(GetRevitDocument());

        Dictionary<string, int> familyCounts = [];

        foreach (Family family in familyCollector)
        {
            ICollection<ElementId> familyTypeIds = family.GetFamilySymbolIds();
            var count = 0;

            foreach (var typeId in familyTypeIds)
                count += new FilteredElementCollector(GetRevitDocument())
                    .OfCategoryId(family.FamilyCategory.Id)
                    .WhereElementIsNotElementType()
                    .Where(e => e.GetTypeId() == typeId)
                    .ToList()
                    .Count;

            familyCounts[family.Name] = count;
        }

        var d = new Dictionary<string, object>
        {
            ["familyCount"] = familyCounts
        };
        // SendCatchToCallback(new Prey(d));
        return true;
    }
}