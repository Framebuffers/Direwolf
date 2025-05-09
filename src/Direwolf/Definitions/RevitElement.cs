using Autodesk.Revit.DB;

namespace Direwolf.Definitions;

public readonly record struct RevitElement(
    string? CategoryType,
    string? CategoryName,
    string? BuiltInCategory,
    double? ElementTypeId,
    string? ElementUniqueId,
    double? ElementId,
    string? ElementName)
{
    public static RevitElement Create(Document doc, ElementId id)
    {
        var element = doc.GetElement(id);
        var elementTypeId = doc.GetElement(id).GetTypeId();
        var category = GetCategory(element);
        
        return new RevitElement(
            category.CategoryType,
            category.CategoryName,
            category.BuiltInCategory,
            elementTypeId.Value,
            element?.UniqueId,
            element?.Id.Value,
            element?.Name ?? string.Empty);
    }

    private static (string? CategoryType, string? CategoryName, string? BuiltInCategory) GetCategory(Element e)
    {
        var category = e?.Category;
        return category is null 
            ? (null,null, null) 
            : (category.CategoryType.ToString(), category.Name, category.BuiltInCategory.ToString());
    }
}