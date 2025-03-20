using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Direwolf.Revit.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Direwolf.Revit.ElementFilters
{
    public static class DocumentExtensions
    {
        public static IEnumerable<Element?> GetAllValidElements(this Document doc)
        {
            using FilteredElementCollector collector = new FilteredElementCollector(doc).WhereElementIsNotElementType();
            foreach (var e in collector)
            {
                if (e is not null && e.IsValidObject && e.Category is not null && e?.Category?.CategoryType is not CategoryType.Invalid || e?.Category?.CategoryType is not CategoryType.Internal)
                    yield return e;
            }
        }

        public static IEnumerable<Element?> GetAnnotativeElements(this Document doc)
        {
            foreach (var e in from e in doc.GetAllValidElements()
                              where e.Category is not null
                              where e.Category.CategoryType is CategoryType.Annotation
                              select e)
            {
                yield return e;
            }
        }

        public static IEnumerable<Element?> GetDesignOptions(this Document doc)
        {
            foreach (var e in from e in doc.GetAllValidElements()
                              where e is DesignOption
                              where e.Category.BuiltInCategory is BuiltInCategory.OST_DesignOptions
                              select e)
            {
                yield return e;
            }
        }

        public static IEnumerable<Element?> GetDetailGroups(this Document doc)
        {
            foreach (var e in from e in doc.GetAllValidElements()
                              where e is Group
                              where e.Category is not null
                              where e.Category.Name == "Detail Groups"
                              select e)
            {
                yield return e;
            }
        }
        public static IEnumerable<Element?> GetModelGroups(this Document doc)
        {
            foreach (var e in from x in doc.GetAllValidElements()
                              where x is Group
                              where x.Category is not null
                              where x.Category.Name != "Detail Groups"
                              select x)
            {
                yield return e;
            }
        }

        public static Dictionary<string, int> GetElementsByWorkset(this Document doc)
        {

            Dictionary<string, int> worksetElementCount = [];
            foreach (string? worksetName in from Element element in doc.GetAllValidElements()
                                            let worksetParam = element.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM)
                                            where worksetParam != null
                                            let worksetName = worksetParam.AsValueString()
                                            select worksetName)
            {
                worksetElementCount[worksetName] =
                    worksetElementCount.TryGetValue(worksetName, out int value)
                    ? value++
                    : 1;
            }
            return worksetElementCount;
        }

        public static IDictionary<string, object>? GetSharedParameterValue(this Element e)
        {
            foreach (Parameter p in e.GetOrderedParameters())
            {
                if (p.IsShared) return p.GetParameterValue();
                else continue;
            }
            return null;
        }

        public static IDictionary<string, object>? GetParametersFromElement(this Element e)
        {
            foreach (Parameter p in e.GetOrderedParameters())
            {
                return new Dictionary<string, object>()
                {

                    ["guid"] = p.GUID,
                    ["hasValue"] = p.HasValue,
                    ["id"] = p.Id.Value,
                    ["isReadOnly"] = p.IsReadOnly,
                    ["isShared"] = p.IsShared,
                    ["isUserModifiable"] = p.UserModifiable,
                };
            }
            return null;
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
                        parameters.Add("value", p.AsInteger());
                        break;
                    case StorageType.Double:
                        parameters.Add("storageType", StorageType.Double.ToString());
                        parameters.Add("value", p.AsDouble());
                        break;
                    case StorageType.String:
                        parameters.Add("storageType", StorageType.String.ToString());
                        parameters.Add("value", p.AsString());
                        break;
                    case StorageType.ElementId:
                        parameters.Add("storageType", StorageType.ElementId.ToString());
                        parameters.Add("value", p.AsElementId());
                        break;
                    case StorageType.None:
                    default:
                        break;
                }
            }
            return parameters;
        }

        public static Dictionary<Family, int> GetInstancesPerFamily(this Document doc)
        {
            Dictionary<Family, int> results = [];
            foreach (Family family in doc.GetFamilies())
            {
                results.Add(family, family.GetFamilySymbolIds().Count);
            }
            return results;
        }

        public static Dictionary<Category, List<Family>> GetFamiliesByCategory(this Document doc)
        {
            var elementsSortedByFamily = new Dictionary<Category, List<Family>>();
            foreach ((Element? f, Category? familyCategory) in from Element e in doc.GetAllValidElements()
                                                               let f = e as FamilyInstance
                                                               where f is not null
                                                               let familyCategory = f.Symbol.Family.Category
                                                               where familyCategory is not null
                                                               select (e, familyCategory))
            {
                if (!elementsSortedByFamily.TryGetValue(familyCategory, out List<Family>? value))
                {
                    value = [];
                    elementsSortedByFamily[familyCategory] = value;
                }
                value.Add((Family)f);
            }
            return elementsSortedByFamily;

        }

        public static Dictionary<Family, List<Element?>> GetElementsByFamily(this Document doc)
        {
            var elementsSortedByFamily = new Dictionary<Family, List<Element?>>();
            foreach ((Element e, Family familyName) in from Element e in doc.GetAllValidElements()
                                                       let f = e as FamilyInstance
                                                       where f is not null
                                                       let familyName = f.Symbol.Family
                                                       select (e, familyName))
            {
                if (!elementsSortedByFamily.TryGetValue(familyName, out List<Element?>? value))
                {
                    value = [];
                    elementsSortedByFamily[familyName] = value;
                }
                value.Add(e);
            }
            return elementsSortedByFamily;
        }

        public static IEnumerable<FailureMessage> GetErrorsAndWarnings(this Document doc) => doc.GetWarnings();

        public static IEnumerable<Family> GetFamilies(this Document doc)
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

        public static int GetGridLineCount(this Document doc)
        {
            return doc.GetAllValidElements()
                .OfType<Grid>()
                .Where(x => x.Category is not null && x.Category.BuiltInCategory is BuiltInCategory.OST_Grids)
                .Count();
        }

        public static IEnumerable<Element?> GetExternalFileReferences(this Document doc)
        {
            foreach (var e in from x in doc.GetAllValidElements()
                              where x is not null
                              where x.IsExternalFileReference()
                              select x)
            {
                yield return e;
            }
        }

        public static IEnumerable<Element?> GetInPlaceFamilies(this Document doc)
        {
            foreach (var e in from x in doc.GetFamilies()
                              where x.IsInPlace
                              select x)
            {
                yield return e;
            }
        }

        public static IEnumerable<Family> GetFamliesWithMostTypes(this Document doc)
        { 
            return doc.GetFamilies().OrderByDescending(x => x.GetFamilySymbolIds().Count);
        }

        public static int GetLevelCount(this Document doc)
        {
            return doc.GetAllValidElements().OfType<Grid>().Count();
        }

        public static IEnumerable<Element?> GetMirroredObjects(this Document doc)
        {
            foreach (var e in from x in doc.GetAllValidElements()
                              where x is FamilyInstance
                              let m = x as FamilyInstance
                              where m.Mirrored
                              select m)
            {
                yield return e;
            }
        }


        public static IEnumerable<Element?> GetNonNativeObjectStyles(this Document doc)
        {
            foreach (var x in from e in doc.GetAllValidElements()
                              where e.Category is not null
                              where e.Category.IsCuttable
                              where e.Category.CategoryType is CategoryType.Annotation
                              select e)
            {
                yield return x;
            }
        }

        public static IEnumerable<Element?> GetUnconnectedDucts(this Document doc)
        {
            foreach (var e in from e in doc.GetAllValidElements()
                              where e is not null
                              where e.Category is not null
                              where e.Category.BuiltInCategory is BuiltInCategory.OST_DuctCurves
                              let d = e as Duct
                              where d is not null
                              let connector = d.ConnectorManager.Connectors
                              from Connector c in connector
                              where !c.IsConnected
                              select e)
            {
                yield return e;
            }
        }

        public static IEnumerable<Element?> GetUnconnectedElectrical(this Document doc)
        {

            foreach (var e in from e in doc.GetAllValidElements()
                              where e is not null
                              where e.Category is not null
                              where e.Category.BuiltInCategory is BuiltInCategory.OST_ElectricalFixtures
                              let mep = ((FamilyInstance)e).MEPModel
                              where mep is not null
                              let connectors = mep.ConnectorManager.Connectors
                              from Connector c in connectors
                              where !c.IsConnected
                              select e)
            {
                yield return e;
            }
        }


        public static IEnumerable<Element?> GetUnconnectedPipes(this Document doc)
        {
            foreach (var e in from e in doc.GetAllValidElements()
                              where e is not null
                              where e.Category is not null
                              where e.Category.BuiltInCategory is BuiltInCategory.OST_PipeCurves
                              let p = e as Pipe
                              where p is not null
                              let c = p.ConnectorManager.Connectors
                              from Connector cn in c
                              where !cn.IsConnected
                              select e)
            {
                yield return e;
            }
        }

        public static IEnumerable<Element?> GetUnenclosedRooms(this Document doc)
        {
            foreach (var x in from e in doc.GetAllValidElements()
                              where e.Category is not null
                              where e.Category.BuiltInCategory is BuiltInCategory.OST_Rooms
                              let room = e as Room
                              let boundaries = room.GetBoundarySegments(new SpatialElementBoundaryOptions())
                              where boundaries is null
                              where boundaries.Count == 0
                              select e)
            {
                yield return x;
            }
        }

        public static IEnumerable<Element?> GetUnusedFamilies(this Document doc)
        {
            foreach (var x in from e in doc.GetFamilies()
                              where e.GetFamilySymbolIds().Count == 0
                              select e)
            {
                yield return x;
            }
        }

        public static IEnumerable<Element?> GetViews(this Document doc)
        {

            foreach (var x in from e in doc.GetAllValidElements()
                              where e.Category is not null
                              where e.Category.BuiltInCategory is BuiltInCategory.OST_Views
                              where e is View
                              let v = e as View
                              where v is not null
                              where !v.IsTemplate
                              select v)
            {
                yield return x;
            }
        }

        public static IEnumerable<Element?> GetViewsNotInSheets(this Document doc)
        {
            List<View> views = [];
            List<ElementId> viewports = [];

            views.AddRange(from e in doc.GetAllValidElements()
                           where e is View && e is not null
                           let v = e as View
                           where !v.IsTemplate
                           select v);

            viewports.AddRange(from e in doc.GetAllValidElements()
                               where e is Viewport && e is not null
                               let v = e as Viewport
                               let id = v.ViewId
                               select id);

            foreach (var e in from e in views
                              where !viewports.Contains(e.Id)
                              select e)
            {
                yield return e;
            }
        }
        
        public static SortedDictionary<long, string> GetFamilyFileNamesSortedByFileSize(this Document doc)
        {
            try
            {
                string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string folderName = $"dw_test_{DateTime.UtcNow.ToFileTimeUtc()}";
                string fullPath = Path.Combine(appdata, folderName);

                Directory.CreateDirectory(fullPath);
                SortedDictionary<long, string> sorted = [];
                foreach (var (f, rfa) in from Family f in doc.GetFamilies()
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
    }
}
