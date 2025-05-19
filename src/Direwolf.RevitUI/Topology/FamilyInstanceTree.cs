using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Direwolf.Sources.InternalDB;

namespace Direwolf.RevitUI.Topology;

[Transaction(TransactionMode.Manual)]
public class FamilyInstanceTree : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        try
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            Database db = new(doc);
            db.PopulateDatabase();
            TaskDialog.Show("Elements in DB",
                            db.GetDatabaseCount().ToString());

            return Result.Succeeded;
        }
        catch (Exception e) { return Result.Failed; }
    }

    private static IEnumerable<Element>? GetAllValidElements(Document doc)
    {
        ICollection<ElementId>? collector = new FilteredElementCollector(doc)
                                            .WhereElementIsNotElementType()
                                            .ToElementIds();

        foreach (var e in
                 from x in collector
                 let y = doc.GetElement(x)
                 select y)
            if (e is
                {
                    IsValidObject : true,
                    Category.CategoryType: not CategoryType.Invalid
                })
                yield return e;
    }

    public static Dictionary<string, int> GetInstancesPerFamily(Document doc)
    {
        IEnumerable<Element>? validElements = GetAllValidElements(doc);
        Dictionary<string, int> counter = [];
        foreach ((var e, string? c) in
                 from e in validElements
                 let c = GetFamilyName(e)
                 select (e, c))
            if (counter.ContainsKey(c))
                counter[c]++;
            else
                counter[c] = 1;

        return counter;
    }

    private static string GetFamilyName(Element element)
    {
        return element is ElementType elementType
            ? elementType.FamilyName
            : string.Empty;
    }
}