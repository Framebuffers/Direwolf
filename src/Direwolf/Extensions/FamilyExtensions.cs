using System.Data.SqlTypes;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Extensions;
public static class FamilyExtensions
{
    public static List<ElementId?> GetRevitDatabase(this Document doc)
    {
        using StringWriter sw = new();
        var db = new FilteredElementCollector(doc)
            .WhereElementIsNotElementType()
            .ToElementIds();
        
        foreach (var e in db)
        {
            sw.Write(RevitElement.Create(doc, e));
        }
        
        Debug.Print(sw.ToString());
        return db.ToList();
    }
}