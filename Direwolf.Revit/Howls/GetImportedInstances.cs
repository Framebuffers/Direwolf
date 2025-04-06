using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls;

public record GetImportedInstances : RevitHowl
{
    public GetImportedInstances(Document doc)
    {
        SetRevitDocument(doc);
    }

    public override bool Hunt()
    {
        ICollection<Element> importedImages = new FilteredElementCollector(GetRevitDocument())
            .OfClass(typeof(ImportInstance))
            .WhereElementIsNotElementType()
            .ToList();

        var results = importedImages.Select(element => element.Id.Value.ToString()).ToList();

        var d = new Dictionary<string, object>
        {
            ["importedImages"] = results
        };
        SendCatchToCallback(new Prey(d));
        return true;
    }
}