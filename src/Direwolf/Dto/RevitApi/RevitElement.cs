using Autodesk.Revit.DB;

using Direwolf.Dto.Parser;

namespace Direwolf.Dto.RevitApi;

public readonly record struct RevitElement(
    Cuid    Id,
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

        (string CategoryType, string CategoryName, string BuiltInCategory) category = GetCategory(element)!;

        return new RevitElement(Cuid.Create(),
                                category.CategoryType,
                                category.CategoryName,
                                category.BuiltInCategory,
                                elementTypeId.Value,
                                element?.UniqueId,
                                element?.Id.Value,
                                element?.Name ?? string.Empty);
    }

    private static(string?, string? Name, string?) GetCategory(Element e)
    {
        var category = e?.Category;

        return category is null
            ? (null, null, null)
            : (category.CategoryType.ToString(), category.Name, category.BuiltInCategory.ToString());
    }
}