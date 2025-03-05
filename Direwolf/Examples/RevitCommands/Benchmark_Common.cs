using Autodesk.Revit.DB;
using System.Text.Json;

namespace Direwolf.Examples.RevitCommands
{
    internal static class Benchmark_Common
    {
        internal static ICollection<Element> GetAllValidElements(Document doc) => new FilteredElementCollector(doc)
                    .WhereElementIsNotElementType()
                    .WhereElementIsViewIndependent()
                    .ToElements();

        internal static Dictionary<string, object> ProcessParameterMap(Element element)
        {
            try
            {
                Dictionary<string, object> results = [];
                ParameterSet ps = element.Parameters;
                foreach (Parameter p in ps)
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
            catch
            {
                return new Dictionary<string, object>();
            }

        }
        
        internal static void WriteToFile(string filename, object obj)
        {
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), filename);
            File.WriteAllText(fileName, JsonSerializer.Serialize(obj));
        }

        internal static Dictionary<string, object> ExtractElementData(Element element) => new(new Dictionary<string, object>
        {
            [element.Id.ToString()] = new Dictionary<string, object>()
            {
                ["UniqueId"] = element.UniqueId ?? 0.ToString(),
                ["VersionGuid"] = element.VersionGuid.ToString(),
                ["IsPinned"] = element.Pinned.ToString(),
                ["Data"] = Benchmark_Common.ProcessParameterMap(element)
            }
        });

    }
}
