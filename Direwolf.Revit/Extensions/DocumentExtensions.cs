using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;

namespace Direwolf.Revit.Extensions
{
    public static class DocumentExtensions
    {
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

        public static int GetSaveCount(this Document document)
        {
            BasicFileInfo f = BasicFileInfo.Extract(document.PathName);
            return f.GetDocumentVersion().NumberOfSaves;
        }

        public static string GetCurrentDocumentGuid(this Document doc)
        {
            BasicFileInfo f = BasicFileInfo.Extract(doc.PathName);
            return f.GetDocumentVersion().VersionGUID.ToString();
        }


        private static Dictionary<string, List<string>> GetElementUniqueIdByBuiltInCategory(this Document doc, BuiltInCategory b)
        {
            var elements = new Dictionary<string, List<string>>();
            IEnumerable<(string Name, string UniqueID)> pairs =
                            new FilteredElementCollector(doc)
                            .OfCategory(b)
                            .ToElements()
                            .Select(x => (x.Name, x.UniqueId));
            foreach ((string Name, string UniqueID) in pairs)
            {
                if (!elements.TryGetValue(Name, out List<string>? value))
                {
                    elements[Name] = [UniqueID];
                }
                else
                {
                    value.Add(UniqueID);
                }
            }
            return elements;
        }

        public static Dictionary<string, List<string>> GetAllViewsInDocument(this Document doc)
        {
            var annotativeElements = new Dictionary<string, List<string>>();
            IEnumerable<(string Name, string UniqueID)> pairs =
                new FilteredElementCollector(doc)
                .OfClass(typeof(ViewDrafting))
                .ToElements()
                .Select(x => (x.Name, x.UniqueId));

            foreach ((string Name, string UniqueID) in pairs)
            {
                if (!annotativeElements.TryGetValue(Name, out List<string>? value))
                {
                    annotativeElements[Name] = [UniqueID];
                }
                else
                {
                    value.Add(UniqueID);
                }
            }
            return annotativeElements;
        }

        public static Dictionary<string, List<string>> GetAllDesignOptionsInDocument(this Document doc)
        {
            var elements = new Dictionary<string, List<string>>();
            IEnumerable<(string Name, string UniqueID)> pairs =
                new FilteredElementCollector(doc)
                .OfClass(typeof(DesignOption))
                .ToElements()
                .Select(x => (x.Name, x.UniqueId));

            foreach ((string Name, string UniqueID) in pairs)
            {
                if (!elements.TryGetValue(Name, out List<string>? value))
                {
                    elements[Name] = [UniqueID];
                }
                else
                {
                    value.Add(UniqueID);
                }
            }
            return elements;
        }

        public static Dictionary<string, List<string>> GetAllDetailGroupsInDocument(this Document doc)
        {
            return GetElementUniqueIdByBuiltInCategory(doc, BuiltInCategory.OST_IOSDetailGroups);
        }

        public static IEnumerable<Element?> _GetModelGroups(this Document doc)
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

        public static Dictionary<string, int> _GetElementsByWorkset(this Document doc)
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

        public static Dictionary<Category, List<Family>> _GetFamiliesByCategory(this Document doc)
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


        public static IEnumerable<FailureMessage> _GetErrorsAndWarnings(this Document doc) => doc.GetWarnings();

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

        public static int GetGridLineCount(this Document doc)
        {
            return doc.GetAllValidElements()
                .OfType<Grid>()
                .Where(x => x.Category is not null && x.Category.BuiltInCategory is BuiltInCategory.OST_Grids)
                .Count();
        }

        public static IEnumerable<Element?> _GetExternalFileReferences(this Document doc)
        {
            foreach (var e in from x in doc.GetAllValidElements()
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
            return doc.GetAllValidElements().OfType<Grid>().Count();
        }

        public static IEnumerable<Element?> _GetMirroredObjects(this Document doc)
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


        public static IEnumerable<Element?> _GetNonNativeObjectStyles(this Document doc)
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

        public static IEnumerable<Element?> _GetUnconnectedDucts(this Document doc)
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

        public static IEnumerable<Element?> _GetUnconnectedElectrical(this Document doc)
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


        public static IEnumerable<Element?> _GetUnconnectedPipes(this Document doc)
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

        public static IEnumerable<Element?> _GetUnenclosedRooms(this Document doc)
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

        public static IEnumerable<Element?> _GetViewsNotInSheets(this Document doc)
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
