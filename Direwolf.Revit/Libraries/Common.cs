using Autodesk.Revit.DB;
using Direwolf.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Direwolf.Revit.Libraries
{
    internal static class Common
    {
        internal static ICollection<Element>? GetAllValidElements(Autodesk.Revit.DB.Document doc)
        {
            return new FilteredElementCollector(doc)
                    .WhereElementIsNotElementType()
                    .WhereElementIsViewIndependent()
                    .ToElements();
        }

        internal static Dictionary<string, object> ExtractParameterMap(Autodesk.Revit.DB.Element e)
        {
            Dictionary<string, object> results = [];
            ParameterSet ps = e.Parameters;
            foreach (Autodesk.Revit.DB.Parameter p in ps)
            {
                string GetValue()
                {
                    return p.StorageType switch
                    {
                        StorageType.None => "None",
                        StorageType.Integer => p.AsInteger().ToString(),
                        StorageType.Double => p.AsDouble().ToString(),
                        StorageType.String => p.AsString(),
                        StorageType.ElementId => p.AsElementId().ToString(),
                        _ => "None",
                    };
                }

                Dictionary<string, string> data = new()
                {
                    ["GUID"] = p.GUID.ToString(),
                    ["Type"] = p.GetTypeId().TypeId,
                    ["HasValue"] = p.HasValue.ToString(),
                    ["Value"] = GetValue()

                };
                results.Add(p.Definition.Name, results);
            }
            return results;
        }

        internal static Dictionary<string, object> ExtractElementData(Element element) => new(new Dictionary<string, object>
        {
            [element.Id.ToString()] = new Dictionary<string, object>()
            {
                ["UniqueId"] = element.UniqueId ?? 0.ToString(),
                ["VersionGuid"] = element.VersionGuid.ToString(),
                ["IsPinned"] = element.Pinned.ToString(),
                ["Data"] = ExtractParameterMap(element)
            }
        });

    }
}
