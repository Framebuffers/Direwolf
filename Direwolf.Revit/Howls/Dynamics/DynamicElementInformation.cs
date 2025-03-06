using Autodesk.Revit.DB;
using Direwolf.Definitions.Dynamics;
using Microsoft.Scripting.Utils;
using System.Dynamic;

namespace Direwolf.Revit.Howls.Dynamics
{
    public record class DynamicElementInformation : DynamicRevitHowl
    {
        public DynamicElementInformation(Document doc) => SetRevitDocument(doc);

        private static DynamicCatch ProcessParameterMap(Element element)
        {
            try
            {
                dynamic results = new ExpandoObject();
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
                return new DynamicCatch(results);
            }
            catch
            {
                return new DynamicCatch();
            }
        }

        private static DynamicCatch ExtractElementData(Element element)
        {
            dynamic x = new ExpandoObject();
            x.UniqueId = element.UniqueId ?? 0.ToString();
            x.VersionGuid = element.VersionGuid.ToString();
            x.IsPinned = element.Pinned.ToString();
            x.Data = ProcessParameterMap(element);
            return new DynamicCatch(x);
        }

        public override bool Execute()
        {
            try
            {
                dynamic Catches = new ExpandoObject();
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
                    List<DynamicCatch> elementData = [];
                    elementData.AddRange(family.Value.Select(ExtractElementData));

                    if (Catches.TryGetValue(family.Key, out List<DynamicCatch>? existingElementData))
                    {
                        existingElementData?.AddRange(elementData);
                    }
                    else
                    {
                        Catches[family.Key] = elementData;
                    }
                }

                dynamic final = new ExpandoObject();
                final.ElementData = Catches;
                SendCatchToCallback(new DynamicCatch(final));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

