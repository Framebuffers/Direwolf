using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Direwolf.Revit.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Direwolf.Revit.Extensions
{
    /* 
     * Author: Framebuffer
     * Date  : 2025-03-20
     * Direwolf Query Architecture
     * 
     * I need to explain a couple of things to *understand* some of my decisions within Direwolf.
     * 
     * To get Elements from Revit, there's an API feature called FilteredElementCollector (and others).
     * They make it super easy (and efficient) to get data back and forth Revit.
     * They use a small record attached to each Element to iterate quickly on each. 
     * These are called FastFilters. Those who need to open the whole Element to check for some condition are called SlowFilters.
     * 
     * They're great to run any kind of *specific* check.
     * However, for the main Reaper, there's a problem: there's several checks, sortings and conditionals to run *for each Element*.
     * Running a Filter for each condition would iterate through the entire record DB: even if it's fast, it's unnecessary.
     * 
     * This is why the main Reaper has an ugly 400+ LOC function to sieve filters and sort them *manually*.
     * These extensions take some of those checks and make them actually usable.
     * 
     * One more thing: but Frame, what does Direwolf do for me? This is where Introspection comes.
     * I know from personal experience what we needed and we didn't down in the modelling battleground. 
     * It is *super* easy to reap data for data's sake. In fact, one approach is to serialise the *entire* document to Prey and send them to a PostgreSQL database. You can do that. There's a Howl for that.
     * 
     * But I don't want to do that. This is what I'm going to do:
     * The thing I did the most was filling Parameters: how many were empty, how many were full, how much of something, how long until it's done... 
     * And, apart from that, the secondary goal is making the *more annoying* sides of Revit less so. 
     * Yes, things like View/Object Styles, checking if there were some hidden Element that was clogging the model, checking if the drawing was modelled or it was just made of lines and fills-- you know, because, again, some *common operations* for us modellers are just outright hostile.
     * 
     * Introspection works by querying Elements in such a way where Revit gives a valid range of Elements, Direwolf sieves them by: CategoryType, Category, Family, Element, a Dictionary of Parameters. Direwolf *abstracts* these to records, just like Revit, to the important bits, whilst also leaving room to add more.
     * 
     * Documents are extended with ready-to-go Filters for the most common operations, like the ones described above. Elements are extended with a method to get the Element's important info as a special kind of Prey, tailor-made to get just the important bits-- it's just like Revit, but applied to this specific use case.
     * 
     * There are some legacy filters that may or may not work. These start with an underscore (_). The main Reaper will still work just like it was when I first started this project, but this should fix some bugs with invalid queries.
     * 
     */

    public static class DocumentExtensions
    {

        public static FilteredElementCollector GetElementsByType(this Document doc, ElementClassFilter f)
        {
            return new FilteredElementCollector(doc).WherePasses(f);
        }



        public static IEnumerable<Element?> _GetAllValidElements(this Document doc)
        {



            using FilteredElementCollector collector = new FilteredElementCollector(doc).WhereElementIsNotElementType();
            foreach (var e in collector)
            {
                if (e is not null && e.IsValidObject && e.Category is not null && e?.Category?.CategoryType is not CategoryType.Invalid || e?.Category?.CategoryType is not CategoryType.Internal)
                    yield return e;
            }
        }

        public static IEnumerable<Element?> _GetAnnotativeElements(this Document doc)
        {

            foreach (var e in from e in doc._GetAllValidElements()
                              where e.Category is not null
                              where e.Category.CategoryType is CategoryType.Annotation
                              select e)
            {
                yield return e;
            }
        }

        public static IEnumerable<Element?> _GetDesignOptions(this Document doc)
        {
            foreach (var e in from e in doc._GetAllValidElements()
                              where e is DesignOption
                              where e.Category.BuiltInCategory is BuiltInCategory.OST_DesignOptions
                              select e)
            {
                yield return e;
            }
        }

        public static IEnumerable<Element?> _GetDetailGroups(this Document doc)
        {
            foreach (var e in from e in doc._GetAllValidElements()
                              where e is Group
                              where e.Category is not null
                              where e.Category.Name == "Detail Groups"
                              select e)
            {
                yield return e;
            }
        }
        public static IEnumerable<Element?> _GetModelGroups(this Document doc)
        {
            foreach (var e in from x in doc._GetAllValidElements()
                              where x is Group
                              where x.Category is not null
                              where x.Category.Name != "Detail Groups"
                              select x)
            {
                yield return e;
            }
        }

        public static Dictionary<string, int> _GetElementsByWorkset(this Document doc)
        {

            Dictionary<string, int> worksetElementCount = [];
            foreach (string? worksetName in from Element element in doc._GetAllValidElements()
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

        public static IDictionary<string, object>? _GetSharedParameterValue(this Element e)
        {
            foreach (Parameter p in e.GetOrderedParameters())
            {
                if (p.IsShared) return p._GetParameterValue();
                else continue;
            }
            return null;
        }

        public static IDictionary<string, object>? _GetParametersFromElement(this Element e)
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

        public static Dictionary<string, object>? _GetParameterValue(this Parameter p)
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

        public static Dictionary<Family, int> _GetInstancesPerFamily(this Document doc)
        {
            Dictionary<Family, int> results = [];
            foreach (Family family in doc._GetFamilies())
            {
                results.Add(family, family.GetFamilySymbolIds().Count);
            }
            return results;
        }

        public static Dictionary<Category, List<Family>> _GetFamiliesByCategory(this Document doc)
        {
            var elementsSortedByFamily = new Dictionary<Category, List<Family>>();
            foreach ((Element? f, Category? familyCategory) in from Element e in doc._GetAllValidElements()
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

        public static Dictionary<Family, List<Element?>> _GetElementsByFamily(this Document doc)
        {
            var elementsSortedByFamily = new Dictionary<Family, List<Element?>>();
            foreach ((Element e, Family familyName) in from Element e in doc._GetAllValidElements()
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

        public static IEnumerable<FailureMessage> _GetErrorsAndWarnings(this Document doc) => doc.GetWarnings();

        public static IEnumerable<Family> _GetFamilies(this Document doc)
        {
            foreach (var e in from x in doc._GetAllValidElements()
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
            return doc._GetAllValidElements()
                .OfType<Grid>()
                .Where(x => x.Category is not null && x.Category.BuiltInCategory is BuiltInCategory.OST_Grids)
                .Count();
        }

        public static IEnumerable<Element?> _GetExternalFileReferences(this Document doc)
        {
            foreach (var e in from x in doc._GetAllValidElements()
                              where x is not null
                              where x.IsExternalFileReference()
                              select x)
            {
                yield return e;
            }
        }

        public static IEnumerable<Element?> _GetInPlaceFamilies(this Document doc)
        {
            foreach (var e in from x in doc._GetFamilies()
                              where x.IsInPlace
                              select x)
            {
                yield return e;
            }
        }

        public static IEnumerable<Family> _GetFamliesWithMostTypes(this Document doc)
        {
            return doc._GetFamilies().OrderByDescending(x => x.GetFamilySymbolIds().Count);
        }

        public static int _GetLevelCount(this Document doc)
        {
            return doc._GetAllValidElements().OfType<Grid>().Count();
        }

        public static IEnumerable<Element?> _GetMirroredObjects(this Document doc)
        {
            foreach (var e in from x in doc._GetAllValidElements()
                              where x is FamilyInstance
                              let m = x as FamilyInstance
                              where m.Mirrored
                              select m)
            {
                yield return e;
            }
        }


        public static IEnumerable<Element?> _GetNonNativeObjectStyles(this Document doc)
        {
            foreach (var x in from e in doc._GetAllValidElements()
                              where e.Category is not null
                              where e.Category.IsCuttable
                              where e.Category.CategoryType is CategoryType.Annotation
                              select e)
            {
                yield return x;
            }
        }

        public static IEnumerable<Element?> _GetUnconnectedDucts(this Document doc)
        {
            foreach (var e in from e in doc._GetAllValidElements()
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

        public static IEnumerable<Element?> _GetUnconnectedElectrical(this Document doc)
        {

            foreach (var e in from e in doc._GetAllValidElements()
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


        public static IEnumerable<Element?> _GetUnconnectedPipes(this Document doc)
        {
            foreach (var e in from e in doc._GetAllValidElements()
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

        public static IEnumerable<Element?> _GetUnenclosedRooms(this Document doc)
        {
            foreach (var x in from e in doc._GetAllValidElements()
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

        public static IEnumerable<Element?> _GetUnusedFamilies(this Document doc)
        {
            foreach (var x in from e in doc._GetFamilies()
                              where e.GetFamilySymbolIds().Count == 0
                              select e)
            {
                yield return x;
            }
        }

        public static IEnumerable<Element?> _GetViews(this Document doc)
        {

            foreach (var x in from e in doc._GetAllValidElements()
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

        public static IEnumerable<Element?> _GetViewsNotInSheets(this Document doc)
        {
            List<View> views = [];
            List<ElementId> viewports = [];

            views.AddRange(from e in doc._GetAllValidElements()
                           where e is View && e is not null
                           let v = e as View
                           where !v.IsTemplate
                           select v);

            viewports.AddRange(from e in doc._GetAllValidElements()
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
    }
}
