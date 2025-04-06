using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls;

public record GetElementInformation : RevitHowl
{
    public GetElementInformation(Document doc)
    {
        SetRevitDocument(doc);
    }

    private static Prey ProcessParameterMap(Element element)
    {
        try
        {
            Dictionary<string, object> results = [];
            var ps = element.Parameters;
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
                        _ => "None"
                    };
                }

                Dictionary<string, string> data = new()
                {
                    ["id"] = p.GUID.ToString(),
                    ["type"] = p.GetTypeId().TypeId,
                    ["hasValue"] = p.HasValue.ToString(),
                    ["value"] = GetValue()
                };
                results.Add(p.Definition.Name, results);
            }

            return new Prey(results);
        }
        catch
        {
            return new Prey();
        }
    }

    private static Prey ExtractElementData(Element element)
    {
        return new Prey(new Dictionary<string, object>
        {
            [element.Id.ToString()] = new Dictionary<string, object>
            {
                ["uniqueId"] = element.UniqueId ?? 0.ToString(),
                ["versionGuid"] = element.VersionGuid.ToString(),
                ["isPinned"] = element.Pinned.ToString(),
                ["data"] = ProcessParameterMap(element)
            }
        });
    }

    public override bool Hunt()
    {
        try
        {
            Dictionary<string, List<Prey>> catches = [];
            ICollection<Element> allValidElements = new FilteredElementCollector(GetRevitDocument())
                .WhereElementIsNotElementType()
                .WhereElementIsViewIndependent()
                .ToElements();
            Dictionary<string, List<Element>> elementsSortedByFamily = [];

            foreach (var (e, familyName) in from Element e in allValidElements
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
                List<Prey> elementData = [];
                elementData.AddRange(family.Value.Select(ExtractElementData));

                if (catches.TryGetValue(family.Key, out var existingElementData))
                    existingElementData.AddRange(elementData);
                else
                    catches[family.Key] = elementData;
            }

            SendCatchToCallback(new Prey(new Dictionary<string, object>
            {
                ["elementData"] = catches
            }));
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}