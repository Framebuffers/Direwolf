using System.Diagnostics;

using Autodesk.Revit.DB;

using Direwolf.Dto.RevitApi;

namespace Direwolf.Extensions;

public static class FamilyExtensions
{
    public static IEnumerable<RevitElement> GetRevitDatabase(this Document doc)
    {
        return
            from elementId in new FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds()
            let el = RevitElement.Create(doc,
                                         elementId)
            where el.BuiltInCategory != null && el.ElementId!.Value > 0
            select el;
    }

    public static IEnumerable<ElementId> GetElementTypes(this Document doc)
    {
        return
            from elementId in new FilteredElementCollector(doc).WhereElementIsElementType().ToElementIds()
            select elementId;
    }
}