using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls;

public record GetUnusedFamilies : RevitHowl
{
    public GetUnusedFamilies(Document doc)
    {
        SetRevitDocument(doc);
    }

    public override bool Hunt()
    {
        using var familyCollector = new FilteredElementCollector(GetRevitDocument())
            .OfClass(typeof(Family));

        List<string> unusedFamilies = [];

        foreach (var family in familyCollector.Cast<Family>())
            try
            {
                using var instanceCollector = new FilteredElementCollector(GetRevitDocument())
                    .OfCategory(family.Category.BuiltInCategory)
                    .WhereElementIsNotElementType();

                if (!instanceCollector.Any()) unusedFamilies.Add(family.Name);
            }
            catch
            {
            }

        var d = new Dictionary<string, object>
        {
            ["unusedFamilies"] = unusedFamilies
        };
        // SendCatchToCallback(new Prey(d));
        return true;
    }
}