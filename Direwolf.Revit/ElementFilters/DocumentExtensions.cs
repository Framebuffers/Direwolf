using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Direwolf.Definitions;
using Direwolf.Revit.Utilities;
using System.Diagnostics;

namespace Direwolf.Revit.ElementFilters
{
    public static class DocumentExtensions
    {
        public static IEnumerable<Element> GetAllValidElements(this Document doc)
        {
            using FilteredElementCollector collector = new FilteredElementCollector(doc).WhereElementIsNotElementType();
            foreach (var e in collector)
            {
                if (e is not null && e.IsValidObject && e.Category is not null && e?.Category?.CategoryType is not CategoryType.Invalid || e?.Category?.CategoryType is not CategoryType.Internal)
                    yield return e;
            }
        }


        public static IEnumerable<Element> GetAnnotativeElements(this Document doc)
        {
            return [.. doc.GetAllValidElements()
                          .Where(x => x.Category.CategoryType is CategoryType.Annotation)];
        }

        public static IEnumerable<Element> GetDesignOptions(this Document doc)
        {
            return [.. doc.GetAllValidElements()
                          .Where(x => x is DesignOption)
                          .Cast<DesignOption>()];
        }

        public static IEnumerable<Element> GetDetailGroups(this Document doc)
        {
            return [.. doc.GetAllValidElements()
                .Where(x => x is Group)
                .Where(e => e.Category.Name == "Detail Groups")];
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

        public static SortedDictionary<int, Family> GetInstancesPerFamily(this Document doc)
        {
            var elements = doc.GetElementsByFamily();
            SortedDictionary<int, Family> results = [];
            foreach (var v in elements)
            {
                results.Add(v.Value.Count, v.Key);
            }
            return results;
        }

        public static SortedDictionary<Category, List<Family>> GetFamiliesByCategory(this Document doc)
        {
            var elementsSortedByFamily = new SortedDictionary<Category, List<Family>>();
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

        public static Dictionary<Family, List<Element>> GetElementsByFamily(this Document doc)
        {
            var elementsSortedByFamily = new Dictionary<Family, List<Element>>();
            foreach ((Element e, Family familyName) in from Element e in doc.GetAllValidElements()
                                                       let f = e as FamilyInstance
                                                       where f is not null
                                                       let familyName = f.Symbol.Family
                                                       select (e, familyName))
            {
                if (!elementsSortedByFamily.TryGetValue(familyName, out List<Element>? value))
                {
                    value = [];
                    elementsSortedByFamily[familyName] = value;
                }
                value.Add(e);
            }
            return elementsSortedByFamily;
        }

        public static IEnumerable<FailureMessage> GetErrorsAndWarnings(this Document doc) => doc.GetWarnings();

        public static HashSet<Family> GetFamilies(this Document doc)
        {
            return [.. doc.GetAllValidElements()
                          .Where(x => !x.ViewSpecific)
                          .OfType<FamilyInstance>()
                          .Select(x => x.Symbol.Family)];
        }

        public static int GetGridLineCount(this Document doc)
        {
            return doc.GetAllValidElements()
                .OfType<Grid>()
                .Count();
        }

        public static Element[] GetExternalFileReferences(this Document doc)
        {
            return [.. doc.GetAllValidElements().Where(x => x.IsExternalFileReference())];
        }

        public static IEnumerable<Element> GetInPlaceFamilies(this Document doc)
        {
            return [.. doc.GetFamilies().Where(x => x.IsInPlace)];
        }

        public static IEnumerable<Family> GetFamliesWithMostTypes(this Document doc)
        {
            var count = [.. doc.GetFamilies().Select(x => x.GetFamilySymbolIds())];
        }

        public static int GetLevelCount(this Document doc)
        {
            return new FilteredElementCollector(doc)
                .WhereElementIsViewIndependent()
                .WhereElementIsNotElementType()
                .OfClass(typeof(Level))
                .GetElementCount();
        }

        public static IEnumerable<Element> GetMirroredObjects(this Document doc)
        {
            return [.. doc.GetAllValidElements()
                      .OfType<FamilyInstance>()
                      .Where(x => x.GetTransform().HasReflection)];
        }

        public static IEnumerable<Element> GetModelGroups(this Document doc)
        {
            return [.. doc.GetAllValidElements()
                .OfType<Group>()];
        }

        public static IEnumerable<Element> GetNonNativeObjectStyles(this Document doc)
        {
            return [.. new FilteredElementCollector(doc)
                .OfClass(typeof(GraphicsStyle))
                .WhereElementIsNotElementType()
                .WhereElementIsViewIndependent()
                .Where(x => x is not null)
                .Where(x => x.Category is not null && x.Category.IsCuttable)
                .Where(x => x.Category.CategoryType is CategoryType.Annotation)];
        }

        public static IEnumerable<Element> GetImportedInstances(this Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(ImportInstance))
                .WhereElementIsNotElementType()
                .WhereElementIsViewIndependent()
                .ToElements();
        }

        public static IEnumerable<Element> GetUnconnectedDucts(this Document doc)
        {
            var ducts = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_DuctCurves)
                .WhereElementIsNotElementType()
                .WhereElementIsViewIndependent()
                .Cast<Duct>();

            return [.. from Duct d in ducts
                   from ConnectorSet set in d.ConnectorManager.Connectors
                   from Connector c in set
                   where !c.IsConnected
                   select d];
        }

        public static IEnumerable<Element> GetUnconnectedElectrical(this Document doc)
        {
            var electricalCollector =
                new FilteredElementCollector(doc)
                    .WhereElementIsViewIndependent()
                    .WhereElementIsNotElementType()
                    .OfCategory(BuiltInCategory.OST_ElectricalFixtures);

            return from Element electricalElement in electricalCollector
                   let mepModel = ((FamilyInstance)electricalElement).MEPModel
                   where mepModel != null
                   let connectors = mepModel.ConnectorManager.Connectors
                   from Connector connector in connectors
                   where !connector.IsConnected
                   select electricalElement;
        }

        public static IEnumerable<Element> GetUnconnectedPipes(this Document doc)
        {
            IEnumerable<Pipe> pipeCollector = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_PipeCurves)
                .WhereElementIsViewIndependent()
                .WhereElementIsNotElementType()
                .Cast<Pipe>();

            return [.. from Pipe pipeElement in pipeCollector
                       let connectors = pipeElement.ConnectorManager.Connectors
                       from Connector connector in connectors
                       where !connector.IsConnected
                       select pipeElement];

        }

        public static IEnumerable<Element> GetUnenclosedRooms(this Document doc)
        {
            return [.. from room in new FilteredElementCollector(doc)
                         .OfCategory(BuiltInCategory.OST_Rooms)
                         .WhereElementIsViewIndependent()
                         .WhereElementIsNotElementType()
                         .Cast<Room>()
                           let r = room
                           let boundary = r.GetBoundarySegments(new SpatialElementBoundaryOptions())
                       where boundary is null && boundary.Count == 0
                       select room ];
        }

        public static IEnumerable<Element> GetUnusedFamilies(this Document doc)
        {
            return new FilteredElementCollector(doc)
                           .WhereElementIsNotElementType()
                           .WhereElementIsViewIndependent()
                           .OfClass(typeof(Family))
                           .Cast<Family>()
                           .Where(x => x is not null)
                           .Where(x => !x.GetFamilySymbolIds().Any())
                           .AsEnumerable();
        }

        public static IEnumerable<Element> GetViews(this Document doc)
        {
            return [.. from view in new FilteredElementCollector(doc)
                       .WhereElementIsNotElementType()
                       .WhereElementIsViewIndependent()
                       .OfClass(typeof(View))
                       where view is not null
                       let viewCheck = view as View
                       where viewCheck.IsTemplate is false
                       select view];
        }

        public static IEnumerable<Element> GetViewsNotInSheets(this Document doc)
        {
            var viewCollector =
                new FilteredElementCollector(doc)
                       .OfClass(typeof(View))
                       .WhereElementIsViewIndependent()
                       .WhereElementIsNotElementType()
                       .Where(x => x is not null)
                       .Cast<View>();

            HashSet<ElementId> viewportIds =
                [.. new FilteredElementCollector(doc)
                    .OfClass(typeof(Viewport))
                    .WhereElementIsViewIndependent()
                    .WhereElementIsNotElementType()
                    .Where(x => x is not null)
                    .Cast<Viewport>()
                    .Select(x => x.ViewId)];

            return from Element viewElement in viewCollector
                   where viewElement is View view
                   && !view.IsTemplate
                   && !viewportIds.Contains(view.Id)
                   select viewElement;
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

                HashSet<Family> families = [.. new FilteredElementCollector(doc)
                                   .WhereElementIsNotElementType()
                                   .WhereElementIsViewIndependent()
                                   .OfClass(typeof(Family))
                                   .Where(x => x is not null)
                                   .Cast<Family>()
                                   .Where(x => x.IsEditable && x.IsValidObject)
                                   .AsEnumerable()];

                foreach (Family f in families)
                {
                    var rfa = Path.Combine(fullPath, $"{f.Name}.rfa");
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
            catch (Exception e) { Debug.Print(e.Message); }

            throw new Exception($"The routine could not be initialized. Reason: Could not get into the try/catch clause.");
        }
    }
}
