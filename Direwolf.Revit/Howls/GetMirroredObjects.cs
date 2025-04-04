using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls;

public record GetMirroredObjects : RevitHowl
{
    public GetMirroredObjects(Document doc)
    {
        SetRevitDocument(doc);
    }

    public override bool Execute()
    {
        List<FamilyInstance> familyInstances = new FilteredElementCollector(GetRevitDocument())
            .OfClass(typeof(FamilyInstance))
            .WhereElementIsNotElementType()
            .Cast<FamilyInstance>()
            .ToList();

        List<string> mirroredInstances = familyInstances.Where(instance => instance.GetTransform().HasReflection)
            .Select(instance => instance.Id.Value.ToString())
            .ToList();

        var d = new Dictionary<string, object>
        {
            ["mirroredInstances"] = mirroredInstances
        };
        SendCatchToCallback(new Prey(d));
        return true;
    }
}