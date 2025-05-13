using Autodesk.Revit.DB;

namespace Direwolf.Database.Tokens;

public readonly record struct ElementInformation(
    string? CategoryType,
    string? CategoryName,
    string? BuiltInCategory,
    double? ElementTypeId,
    string? ElementUniqueId,
    double? ElementId,
    string? ElementName)
{
    public static ElementInformation Create(Document doc, ElementId id)
    {
        var element = doc.GetElement(id);
        var elementTypeId = doc.GetElement(id).GetTypeId();
        var category = GetCategory(element);
        
        return new ElementInformation(
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