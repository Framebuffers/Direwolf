using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls;

public record GetImportedElementsPath : RevitHowl
{
    public GetImportedElementsPath(Document doc)
    {
        SetRevitDocument(doc);
    }

    public override bool Hunt()
    {
        using FilteredElementCollector filteredElementCollector = new(GetRevitDocument());
        ICollection<Element> importedElements = filteredElementCollector
            .OfClass(typeof(ImportInstance))
            .WhereElementIsNotElementType()
            .ToList();

        var results = new Dictionary<string, string>();
        foreach (var element in importedElements)
        {
            using var pathParam = element.get_Parameter(BuiltInParameter.IMPORT_SYMBOL_NAME);
            var filePath = pathParam != null ? pathParam.AsString() : "Unknown Path";
            results[element.UniqueId] = filePath;
        }

        var d = new Dictionary<string, object>
        {
            ["importedElements"] = results
        };
        SendCatchToCallback(new Prey(d));
        return true;
    }
}