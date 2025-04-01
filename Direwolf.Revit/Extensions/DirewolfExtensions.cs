using Autodesk.Revit.DB;

namespace Direwolf.Revit.Utilities;

/// <summary>
/// Helper extensions to obtain information from a Revit Document, Element or Parameter.
/// </summary>
public static class DirewolfExtensions
{
    public static SortedDictionary<long, string> _GetFamilyFileNamesSortedByFileSize(this Document doc)
    {
        try
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folderName = $"dw_test_{DateTime.UtcNow.ToFileTimeUtc()}";
            string fullPath = Path.Combine(appdata, folderName);

            Directory.CreateDirectory(fullPath);
            SortedDictionary<long, string> sorted = [];
            foreach (var (f, rfa) in from Family f in doc._GetFamilies()
                                     let rfa = Path.Combine(fullPath, $"{f.Name}.rfa")
                                     select (f, rfa))
            {
                try
                {
                    doc.EditFamily(f).SaveAs(rfa);
                    sorted.TryAdd(new FileInfo(rfa).Length, f.Name);
                }
                catch
                {
                    continue;
                }
            }
            return sorted;
        }
        catch { throw new Exception("Error while creating folder"); } // because, else, it could file in the middle
        throw new Exception($"The routine could not be initialized. Reason: Could not get into the try/catch clause.");
    }
    public static IEnumerable<Family> _GetFamilies(this Document doc)
    {
        foreach (var e in from x in doc.GetAllValidElements()
                          where x is FamilyInstance
                          let fi = (FamilyInstance)x
                          where !x.ViewSpecific
                          select fi.Symbol.Family)
        {
            yield return e;
        }
    }

    public static string GetFamilyName(this Element element)
    {
        ElementType? elementType = element as ElementType;
        if (elementType is not null)
        {
            return elementType.FamilyName;
        }
        else
        {
            return string.Empty;
        }
    }

    public static IEnumerable<Element?> GetAllValidElements(this Document doc)
    {
        var collector = new FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds();
        foreach (var e in from x in collector
                          let y = doc.GetElement(x)
                          select y)
        {
            if (e is not null && e.IsValidObject && e.Category is not null && e?.Category?.CategoryType is not CategoryType.Invalid || e?.Category?.CategoryType is not CategoryType.Internal)
                yield return e;
        }
    }

    public static Dictionary<string, int> _GetInstancesPerFamily(this Document doc)
    {
        Dictionary<string, int> counter = [];
        foreach (var (e, c) in from e in doc.GetAllValidElements()
                               let c = e.GetFamilyName()
                               select (e, c))
        {
            if (counter.ContainsKey(c))
            {
                counter[c]++;
            }
            else
            {
                counter[c] = 1;
            }
        }
        return counter;
    }

    public static ICollection<Element>? _GetAllValidElements(this Document doc)
    {
        return new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .WhereElementIsViewIndependent()
                .ToElements();
    }
    public static Dictionary<string, object>? GetParameterValue(this Parameter p)
    {
        Dictionary<string, object> parameters = [];
        if (p.Definition is not null)
        {
            switch (p.StorageType)
            {
                case StorageType.Integer:
                    parameters.Add("storageType", StorageType.Integer.ToString());
                    parameters.Add("name", p.Definition.Name);
                    parameters.Add("value", p.AsInteger());
                    break;
                case StorageType.Double:
                    parameters.Add("storageType", StorageType.Double.ToString());
                    parameters.Add("name", p.Definition.Name);
                    parameters.Add("value", p.AsDouble());
                    break;
                case StorageType.String:
                    parameters.Add("storageType", StorageType.String.ToString());
                    parameters.Add("name", p.Definition.Name);
                    parameters.Add("value", p.AsString());
                    break;
                case StorageType.ElementId:
                    parameters.Add("storageType", StorageType.ElementId.ToString());
                    parameters.Add("name", p.Definition.Name);
                    parameters.Add("value", p.AsElementId().Value);
                    break;
                case StorageType.None:
                default:
                    break;
            }
            parameters.Add("isReadOnly", p.IsReadOnly);
            parameters.Add("typeId", p.GetTypeId().TypeId ?? string.Empty);
            parameters.Add("dataType", p.Definition.GetDataType().TypeId);
            parameters.Add("groupTypeId", p.Definition.GetGroupTypeId().TypeId ?? string.Empty);
            parameters.Add("hasValue", p.HasValue);
            parameters.Add("isShared", p.IsShared);
            parameters.Add("userModifiable", p.UserModifiable);

            if (p.IsShared)
            {
                parameters.Add("sharedParamGuid", p.GUID);
            }
            else
            {
                parameters.Add("sharedParamGuid", Guid.Empty.ToString());
            }
        }
        return parameters;
    }
}
