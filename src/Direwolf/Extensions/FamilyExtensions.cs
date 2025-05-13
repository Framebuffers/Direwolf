using System.Data.SqlTypes;
using System.Diagnostics;
using System.Text.Json;
using Autodesk.Revit.DB;
using Direwolf.Database.Tokens;

namespace Direwolf.Extensions;
public static class FamilyExtensions
{
    public static List<ElementId?> GetRevitDatabase(this Document doc)
    {
        using StringWriter sw = new();
        var db = new FilteredElementCollector(doc)
            .WhereElementIsNotElementType()
            .ToElementIds();

        var records = db
            .Select(e => ElementInformation.Create(doc, e))
            .Where(e => e.BuiltInCategory is not null)
            .ToList();

        foreach (var element in records)
        {
            sw.WriteLine(element.ToString());
        }
        
        Debug.Print(sw.ToString());
        return db.ToList();
    }
}