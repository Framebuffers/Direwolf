using System.Data.SqlTypes;
using System.Diagnostics;
using Autodesk.Revit.DB;

namespace Direwolf.Extensions;

public static class FamilyExtensions
{
    public static List<ElementId?> GetRevitDatabase(this Document doc)
    {
        using StringWriter sw = new StringWriter();
        var  db= new FilteredElementCollector(doc)
            .WhereElementIsNotElementType()
            .ToElementIds();

        foreach (var e in db)
        {
            var element = doc.GetElement(e);
            sw.WriteLine($"Element: {element.Id.Value}::{element.Name ?? string.Empty}");
        }

        var types = new FilteredElementCollector(doc)
            .WhereElementIsElementType()
            .ToElementIds();
        
         foreach (var e in types)
         {
             var element = doc.GetElement(e);
             sw.WriteLine($"ElementType: {element.Id.Value}::{element.Name ?? string.Empty}");
         }
       
        var list = new List<ElementId?>();
        list.AddRange(types);
        list.AddRange(db);
        
        Debug.Print(sw.ToString());
        
        return list;
    }
    
    // public static IEnumerable<Family> GetFamilies(this Document doc)
    // {
    //    
    //     foreach (var e in from x in doc.GetRevitDatabase()
    //              where x is FamilyInstance
    //              let fi = (FamilyInstance)x
    //              where !x.ViewSpecific
    //              select fi.Symbol.Family)
    //         yield return e;
    // } 
    //
    
}