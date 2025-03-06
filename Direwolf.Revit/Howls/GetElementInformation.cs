using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Microsoft.Scripting.Utils;

namespace Direwolf.Revit.Howls
{
    public record class GetElementInformation : RevitHowl
    {
        public GetElementInformation(Document doc) => SetRevitDocument(doc);
        
        private static Catch ProcessParameterMap(Element element)
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
                return new Catch(results);
            }
            catch
            {
                return new Catch();
            }
        }

        private static Catch ExtractElementData(Element element) => new(new Dictionary<string, object>
        {
            [element.Id.ToString()] = new Dictionary<string, object>()
            {
                ["UniqueId"] = element.UniqueId ?? 0.ToString(),
                ["VersionGuid"] = element.VersionGuid.ToString(),
                ["IsPinned"] = element.Pinned.ToString(),
                ["Data"] = ProcessParameterMap(element)
            }
        });

        public override bool Execute()
        {
            try
            {
                Dictionary<string, List<Catch>> Catches = [];
                ICollection<Element> allValidElements = new FilteredElementCollector(GetRevitDocument())
                    .WhereElementIsNotElementType()
                    .WhereElementIsViewIndependent()
                    .ToElements();
                Dictionary<string, List<Element>> elementsSortedByFamily = [];

                foreach ((Element e, string familyName) in from Element e in allValidElements
                                                           let f = e as FamilyInstance
                                                           where f is not null
                                                           let familyName = f.Symbol.Family.Name
                                                           select (e, familyName))
                {
                    if (!elementsSortedByFamily.TryGetValue(familyName, out List<Element>? value))
                    {
                        value = [];
                        elementsSortedByFamily[familyName] = value;
                    }
                    value.Add(e);
                }

                foreach (KeyValuePair<string, List<Element>> family in elementsSortedByFamily)
                {
                    List<Catch> elementData = [];
                    elementData.AddRange(family.Value.Select(ExtractElementData));

                    if (Catches.TryGetValue(family.Key, out List<Catch>? existingElementData))
                    {
                        existingElementData.AddRange(elementData);
                    }
                    else
                    {
                        Catches[family.Key] = elementData;
                    }
                }

               SendCatchToCallback(new Catch(new Dictionary<string, object>()
                {
                    ["ElementData"] = Catches
                }));
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
