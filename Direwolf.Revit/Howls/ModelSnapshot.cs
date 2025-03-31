using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Direwolf.Definitions;
using Direwolf.Extensions;
using Direwolf.Revit.Extensions;
using Direwolf.Revit.Definitions;

namespace Direwolf.Revit.Howls
{
    /// <summary>
    /// Given an <see cref="Element"/>, get the family it belongs to, and return all the parameters for each instance.
    /// Check for overridden parameters.
    /// </summary>
    public record class ModelSnapshot : RevitHowl
    {
        public ModelSnapshot(Document doc)
        {
            SetRevitDocument(doc);
        }

        public override bool Execute()
        {
            try
            {
                if (GetRevitDocument() is not null)
                {
                    // Out of all the elements elementSelected, get the Element, ElementType and ElementId
                    foreach (var (e, type, id) in from selectionFromApp in GetRevitDocument().GetAllValidElements()
                                                                      let elementId = selectionFromApp?.Id
                                                                      let elementSelected = GetRevitDocument().GetElement(elementId)
                                                                      let elementType = elementSelected?.GetTypeId()
                                                                      select (selectionFromApp, elementType, elementId))
                    {
                        if (type is null)
                        {
                            $"No type found".ToConsole();
                            continue;
                        }

                        // Cast the type
                        ElementType? elementType = e?.Document?.GetElement(type) as ElementType;

                        if (e is not null)
                        {
                            // Create a dictionary with the Element's metadata, and then add its parameters.
                            Dictionary<string, object>? element = new()
                            {
                                ["id"] = e.Id.Value,
                                ["uniqueId"] = e.UniqueId,
                                ["familyName"] = elementType?.FamilyName ?? string.Empty,
                                ["elementName"] = e?.Name ?? string.Empty,
                            };

                            List<Dictionary<string, object>> result =
                            [
                                element,
                                new Dictionary<string, object>() { ["parameters"] = GetParameters(e) },
                                GetCategory(e),
                                new Dictionary<string, object>() { ["materials"] = e.GetMaterialIds(true)},
                            ];

                            // Success!
                            SendCatchToCallback(new Prey(result));
                        }
                    }
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private static Dictionary<string, object>? GetCategory(Element e)
        {
            Dictionary<string, object> categoryInfo = [];

            if (e is not null && e.Category is not null)
            {
                categoryInfo.TryAdd("categoryId", e.Category.Id.Value);
                categoryInfo.TryAdd("categoryUniqueId", e.UniqueId ?? Guid.Empty.ToString());
                categoryInfo.TryAdd("categoryName", e.Category.Name ?? string.Empty);
                categoryInfo.TryAdd("builtInCategory", e.Category.BuiltInCategory.ToString() ?? string.Empty);
                categoryInfo.TryAdd("categoryType", e.Category.CategoryType.ToString() ?? string.Empty);
                categoryInfo.TryAdd("hasMaterialQuantities", e.Category.HasMaterialQuantities);
            }
            return categoryInfo;
        }

        private static List<Dictionary<string, object>>? GetParameters(Element e)
        {
            List<Dictionary<string, object>>? results = [];
            if (e is not null && e.GetOrderedParameters() is not null)
            {
                IList<Parameter>? b = e.GetOrderedParameters();
                if (e?.Category is not null)
                {
                    results.AddRange(from p in b
                                     select p.GetParameterValue());
                }
                return results;
            }
            else
            {
                return [];
            }
        }
    }
}
