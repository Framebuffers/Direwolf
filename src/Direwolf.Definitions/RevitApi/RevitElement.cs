using System.Globalization;
using System.Text.Json;

using Autodesk.Revit.DB;

using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.Parser;

namespace Direwolf.Definitions.RevitApi;

public readonly record struct RevitElement(
    Cuid                 Id,
    string?              CategoryType,
    string?              CategoryName,
    long?                BuiltInCategory,
    double?              ElementTypeId,
    string?              ElementUniqueId,
    double?              ElementId,
    string?              ElementName,
    List<RevitParameter> Parameters)
{
    public static RevitElement Create(Document doc, ElementId id)
    {
        var element = doc.GetElement(id);
        var elementTypeId = doc.GetElement(id).GetTypeId();
        element.TryGetParameters(out var param);

        (string CategoryType, string CategoryName, long? BuiltInCategory)
            category = GetCategory(element)!;

        return new RevitElement(Cuid.Create(),
                                category.CategoryType,
                                category.CategoryName,
                                category.BuiltInCategory,
                                elementTypeId.Value,
                                element?.UniqueId,
                                element?.Id.Value,
                                element?.Name ?? string.Empty,
                                param);
    }

    private static(string? CategoryType, string? Name, long? BuiltInCategory)
        GetCategory(Element e)
    {
        var category = e?.Category;

        return category is null
            ? (null, null, null)
            : (category.CategoryType.ToString(), category.Name,
               (long)category.BuiltInCategory);
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(new Dictionary<string, object>()
        {
            [ElementId!.Value.ToString(
                 CultureInfo
                     .InvariantCulture)]
                = new
                    Dictionary<string, object>()
                    {
                        ["Id"] = Id.Value!,
                        ["CategoryType"]
                            = CategoryType
                            ?? string.Empty,
                        ["CategoryName"]
                            = CategoryName
                            ?? string.Empty,
                        ["BuiltInCategory"]
                            = BuiltInCategory
                            ?? -1,
                        ["typeId"]
                            = ElementTypeId
                            ?? -1,
                        ["elementName"]
                            = ElementName
                            ?? string.Empty,
                        ["parameters"]
                            = Parameters
                            ?? null!
                    }
        });
    }
}