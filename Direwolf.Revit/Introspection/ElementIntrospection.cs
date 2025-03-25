using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Direwolf.Definitions;
using Direwolf.Extensions;
using Direwolf.Revit.Extensions;
using Direwolf.Revit.Howls;
using System.Text.Json;

namespace Direwolf.Revit.Introspection
{
    /// <summary>
    /// Given an <see cref="Element"/>, get the family it belongs to, and return all the parameters for each instance.
    /// Check for overridden parameters.
    /// </summary>
    public record class ElementIntrospection : RevitHowl
    {
        public ElementIntrospection(Document doc, UIApplication app)
        {
            SetRevitDocument(doc);
            _app = app;
        }
        private readonly UIApplication? _app;

        public override bool Execute()
        {
            try
            {
                if (_app is not null)
                {
                    var selection = _app.ActiveUIDocument.Selection.GetElementIds();
                    Dictionary<string, object> elementData = [];
                    foreach (var e in selection)
                    {
                        Element? selected = GetRevitDocument().GetElement(e);
                        var elementId = selected?.GetTypeId();
                        if (elementId is null) $"No type found".ToConsole();
                        ElementType? elementType = selected?.Document?.GetElement(elementId) as ElementType;

                        static List<Dictionary<string, object>>? getParameters(Element e)
                        {
                            List<Dictionary<string, object>>? results = [];
                            if (e is not null && e.GetOrderedParameters() is not null)
                            {
                                IList<Parameter>? b = e.GetOrderedParameters();
                                if (e?.Category is not null)
                                {
                                    foreach (var p in b)
                                    {
                                        results.Add(p.GetParameterValue());
                                    }
                                }
                                return results;
                            }
                            else
                            {
                                return new List<Dictionary<string, object>>();
                            }
                        }

                        static Dictionary<string, object>? getCategory(Element e)
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


                        var d = new Dictionary<string, object>()
                        {
                            ["familyName"] = elementType?.FamilyName ?? string.Empty,
                            ["elementName"] = selected?.Name ?? string.Empty,
                            ["elementParameters"] = getParameters(selected),
                        };

                        var category = getCategory(selected);

                        if (category is not null)
                            foreach (var cat in category)
                            {
                                d.Add(cat.Key, cat.Value);
                            }
                        d.TryAdd("materials", selected?.GetMaterialIds(true));
                        SendCatchToCallback(new Prey(d));
                    }

                    //ICollection<ElementId> f = new FilteredElementCollector(GetRevitDocument()).WhereElementIsNotElementType().ToElementIds();

                    //foreach (ElementId id in f)
                    //{
                    //    using StringWriter sw = new();
                    //    try
                    //    {
                    //        var a = GetRevitDocument().GetAllValidElements().LongCount();
                    //        $"{a}".ToConsole();
                    //        Element? selected = GetRevitDocument().GetElement(id);
                    //        var elementId = selected?.GetTypeId();
                    //        if (elementId is null)
                    //        {
                    //            $"No type found".ToConsole();
                    //        }

                    //        ElementType? elementType = selected?.Document?.GetElement(elementId) as ElementType;
                    //        sw.WriteLine($"{elementType?.FamilyName ?? string.Empty}");
                    //        sw.WriteLine($"\tUUID: {elementType?.UniqueId}");
                    //        IList<Parameter>? b = selected?.GetOrderedParameters();
                    //        if (selected?.Category is not null) sw.WriteLine($"BuiltInCategory: \t{elementType?.Category.BuiltInCategory}");
                    //        if (b is not null)
                    //        {
                    //            foreach (var p in b)
                    //            {
                    //                sw.WriteLine($"\t\t{JsonSerializer.Serialize(p.GetParameterValue())}");
                    //            }
                    //        }
                    //        $"{sw}".ToConsole();


                    //    }
                    //    catch { continue; }
                }


                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
