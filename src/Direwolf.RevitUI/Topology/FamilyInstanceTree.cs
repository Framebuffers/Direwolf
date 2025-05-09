using System.Diagnostics;
using System.Runtime.CompilerServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Direwolf.Extensions;

namespace Direwolf.RevitUI.Topology;

[Transaction(TransactionMode.Manual)]
public class FamilyInstanceTree : IExternalCommand
{
    private static IEnumerable<Element?> GetAllValidElements(Document doc)
    {
        var collector = new FilteredElementCollector(doc)
            .WhereElementIsNotElementType()
            .ToElementIds();
        
        foreach (var e in from x in collector
                 let y = doc.GetElement(x)
                 select y)
            if (e is not null && e.IsValidObject && e.Category is not null &&
                 e?.Category?.CategoryType is not CategoryType.Invalid)
                yield return e;
    } 
    
    public static Dictionary<string, int> GetInstancesPerFamily(Document doc)
    {
        var validElements = GetAllValidElements(doc);
        Dictionary<string, int> counter = [];
        foreach (var (e, c) in from e in validElements
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
        return element is ElementType elementType ? elementType.FamilyName : string.Empty;
    }
   
    
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        try
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var result = doc.GetRevitDatabase();
            TaskDialog.Show("FamilyInstances", string.Join("\n", result.Count())); 
            return Result.Succeeded;
        }
        catch (Exception e)
        {
            return Result.Failed;
        }
    }
}