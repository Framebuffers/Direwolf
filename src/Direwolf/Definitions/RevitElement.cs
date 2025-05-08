using Autodesk.Revit.DB;

namespace Direwolf.Definitions;

public readonly record struct RevitElement(
    string? CategoryType,
    string? BuiltInCategory,
    double? ElementTypeId,
    string? ElementUniqueId,
    double? ElementId,
    string? ElementName)
{
    public static RevitElement Create(Document doc, ElementId id)
    {
        var element = doc.GetElement(id);
        var elementTypeId = element?.GetTypeId();
        var category = element?.Category;

        return new RevitElement(
            category?.CategoryType.ToString(),
            category?.BuiltInCategory.ToString(),
            elementTypeId?.Value,
            element?.UniqueId,
            element?.Id.Value,
            element?.Name ?? string.Empty);
    }
}