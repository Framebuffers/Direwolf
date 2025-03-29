using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Direwolf.Definitions;
using Direwolf.Extensions;
using Direwolf.Revit.Extensions;
using Direwolf.Revit.Howls;
using System.Text.Json;
using System.Linq;
using Direwolf.Revit.Definitions.Legacy;
using Direwolf.Revit.Definitions;
using Autodesk.Revit.DB.Mechanical;
using Microsoft.EntityFrameworkCore.Storage;
using Autodesk.Revit.UI.Selection;

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

            if (_app is not null)
            {
                var doc = GetRevitDocument();
                using StringWriter s = new();
                foreach (var x in GetRevitDocument().GetAllValidElements())
                {
                    if (x is not null)
                    {
                        ElementType? type = doc.GetElement(x.GetTypeId()) as ElementType;
                        if (type is not null)
                        {
                            s.WriteLine(type.GetType().DeclaringType);
                            foreach (var t in type.GetSimilarTypes())
                            {
                                s.WriteLine($"\t{t}");
                            }
                        }

                    }

                }
                s.ToString().ToConsole();
                return true;



                //{
                //    foreach ((Element element, ElementId elementType, FamilyInstance familyInstance) in from x in GetRevitDocument().GetAllValidElements()
                //                                                           let t = x?.GetTypeId()
                //                                                           let f = x as FamilyInstance
                //                                                           select (x, t, f))
                //    {
                //        int elementCount = GetRevitDocument().GetAllValidElements().Count();


                //        if (familyInstance is not null)
                //            var similar[] = element?.Document.GetElement(element) as ElementType;
                //            //SendCatchToCallback(new Prey(new Definitions.ElementRecord(GetRevitDocument(), familyInstance)));
                //        }
                //        return true;
                //    }
            }
            return false;
        }


        //public override bool Execute()
        //{
        //    try
        //    {
        //        if (_app is not null)
        //        {
        //            var selection = _app.ActiveUIDocument.Selection.GetElementIds();

        //            if (selection is not null)
        //            {
        //                // Out of all the elements elementSelected, get the Element, ElementType and ElementId
        //                foreach (var (e, selected, elementId) in from selectionFromApp in selection
        //                                                         let elementSelected = GetRevitDocument().GetElement(selectionFromApp)
        //                                                         let elementType = elementSelected?.GetTypeId()
        //                                                         select (selectionFromApp, elementSelected, elementType))
        //                {
        //                    foreach (var x in GetRevitDocument().GetAllValidElements())
        //                    {
        //                        FamilyInstance? f = x as FamilyInstance;
        //                        if (f is not null)
        //                        {
        //                            var doc = GetRevitDocument();
        //                            SendCatchToCallback(new Prey(new ElementRecord(doc, f)));
        //                        }

        //                    }
        //                }
        //                return true;

        //                //    $"No type found".ToConsole();
        //                //    continue;
        //                //}

        //                //// Cast the type
        //                //ElementType? elementType = selected?.Document?.GetElement(elementId) as ElementType;

        //                //if (selected is not null)
        //                //{
        //                //    // Create a dictionary with the Element's metadata, and then add its parameters.
        //                //    Dictionary<string, object>? element = new()
        //                //    {
        //                //        ["id"] = selected.Id.Value,
        //                //        ["uniqueId"] = selected.UniqueId,
        //                //        ["familyName"] = elementType?.ElementFamilyName ?? string.Empty,
        //                //        ["elementName"] = selected?.Name ?? string.Empty,
        //                //    };

        //                //    List<Dictionary<string, object>> result =
        //                //    [
        //                //        element,
        //                //        new Dictionary<string, object>() { ["parameters"] = GetParameters(selected) },
        //                //        GetCategory(selected),
        //                //        new Dictionary<string, object>() { ["materials"] = selected.GetMaterialIds(true)},
        //                //    ];

        //                //    //DocumentIntrospection d = new(GetRevitDocument());
        //                //    //L_ProjectInformationIntrospection pi = new(GetRevitDocument());
        //                //    //L_ProjectSiteIntrospection ps = new(GetRevitDocument());
        //                //    //L_ProjectUnitsIntrospection ui = new(GetRevitDocument());

        //                //    FamilyInstance? f = selected as FamilyInstance;
        //                //    if (f is not null)
        //                //    {
        //                //        SendCatchToCallback(new Prey(new ElementRecord(GetRevitDocument(), f)));
        //                //    }

        //                //    // Success!
        //                //    SendCatchToCallback(new Prey(result));
        //                //    //SendCatchToCallback(new Prey(d));
        //                //    //SendCatchToCallback(new Prey(pi));
        //                //    //SendCatchToCallback(new Prey(ui));
        //                //    //SendCatchToCallback(new Prey(ps));

        //                //        }
        //                //    }
        //                //}
        //                //else
        //            }
        //            return false;
        //        }
        //        return false;

        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
        ////            return true;
        ////        }
        ////        return true;
        ////    }
        ////        return true;
        ////}
        ////    catch
        ////    {
        ////        return false;
        ////    }
        ////}

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
