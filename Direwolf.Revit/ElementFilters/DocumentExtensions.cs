using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using System.Diagnostics;

namespace Direwolf.Revit.ElementFilters
{
    public static class DocumentExtensions
    {
        public static IEnumerable<Element> GetAnnotativeElements(this Document doc)
        {
            return new FilteredElementCollector(doc)
                    .WhereElementIsNotElementType()
                    .Where(e => e.Category != null
                           && e.Category.CategoryType == CategoryType.Annotation);
        }

        public static IEnumerable<Element> GetDesignOptions(this Document doc)
        {
            return new FilteredElementCollector(doc)
                            .WhereElementIsNotElementType()
                            .OfClass(typeof(DesignOption))
                            .Cast<DesignOption>();
        }

        public static IEnumerable<Element> GetDetailGroups(this Document doc)
        {
            return new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfClass(typeof(Group))
                .Cast<Group>()
                .Where(e => e.Category != null && e.Category.Name == "Detail Groups");
        }

        public static Dictionary<string, int> GetElementsByWorkset(this Document doc)
        {

            Dictionary<string, int> worksetElementCount = [];
            foreach (string? worksetName in from Element element in new FilteredElementCollector(doc)
                                                   .WhereElementIsNotElementType()
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

        public static IEnumerable<FailureMessage> GetErrorsAndWarnings(this Document doc) => doc.GetWarnings();

        public static int GetFamilyCount(this Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(Family))
                .GetElementCount();
        }

        public static Dictionary<Family, int> GetFamiliesWithMostInstances(this Document doc)
        {
            using FilteredElementCollector familyCollector = new(doc);
            Dictionary<Family, int> familyCounts = [];

            int count = 0;
            foreach ((Family family, ISet<ElementId> familyTypeIds) in from Family family in familyCollector.Cast<Family>()
                                                                       let familyTypeIds = family.GetFamilySymbolIds()
                                                                       select (family, familyTypeIds))
            {
                foreach (ElementId typeId in familyTypeIds)
                {
                    count += new FilteredElementCollector(doc)
                        .OfCategoryId(family.FamilyCategory.Id)
                        .WhereElementIsNotElementType()
                        .Where(e => e.GetTypeId() == typeId)
                        .ToList()
                        .Count;
                }
                familyCounts[family] = count;
            }
            return familyCounts;
        }

        public static int GetGridLineCount(this Document doc)
        {
            return new FilteredElementCollector(doc)
                 .OfClass(typeof(Grid))
                 .GetElementCount();
        }

        public static IList<Element> GetImportedImages(this Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(ImportInstance))
                .WhereElementIsNotElementType()
                .ToElements();
        }

        public static IEnumerable<Element> GetInPlaceFamilies(this Document doc)
        {
            return new FilteredElementCollector(doc)
                         .OfClass(typeof(FamilyInstance))
                         .Cast<FamilyInstance>()
                         .Select(x => x.Symbol.Family)
                         .Where(x => x.IsInPlace)
                         .AsEnumerable();
        }

        public static IEnumerable<Family> GetFamliesWithMostTypes(this Document doc)
        {
            return (from c in new FilteredElementCollector(doc).OfClass(typeof(Family)).Cast<Family>()
                    orderby c.GetFamilySymbolIds().Count
                    select c).AsEnumerable();
        }

        public static int GetLevelCount(this Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .GetElementCount();
        }

        public static IEnumerable<Element> GetMirroredObjects(this Document doc)
        {
            return [.. new FilteredElementCollector(doc)
                         .OfClass(typeof(FamilyInstance))
                         .WhereElementIsNotElementType()
                         .Cast<FamilyInstance>()
                         .Where(x => x.GetTransform().HasReflection)];
        }

        public static IEnumerable<Element> GetModelGroups(this Document doc)
        {
            return new FilteredElementCollector(doc)
                          .OfClass(typeof(Group))
                          .WhereElementIsNotElementType();
        }

        public static IEnumerable<Element> GetNonNativeObjectStyles(this Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(GraphicsStyle))
                .Where(x => x.Category.IsCuttable && x.Category.CategoryType is CategoryType.Annotation);
        }

        public static IEnumerable<Element> GetImportedInstances(this Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(ImportInstance))
                .WhereElementIsNotElementType()
                .ToElements();
        }

        public static IEnumerable<Element> GetUnconnectedDucts(this Document doc)
        {
            var ducts = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_DuctCurves)
                .WhereElementIsNotElementType()
                .Cast<Duct>();

            return [.. from Duct d in ducts
                   from ConnectorSet set in d.ConnectorManager.Connectors
                   from Connector c in set
                   where !c.IsConnected
                   select d];
        }

        public static IEnumerable<Element> GetUnconnectedElectrical(this Document doc)
        {
            using FilteredElementCollector electricalCollector =
                new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_ElectricalFixtures)
                    .WhereElementIsNotElementType();

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
            using FilteredElementCollector pipeCollector = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_PipeCurves)
                .WhereElementIsNotElementType();

            return [.. from Element pipeElement in pipeCollector
                       where pipeElement is Pipe p
                       let p = pipeElement as Pipe
                       let connectors = p.ConnectorManager.Connectors
                       from Connector connector in connectors
                       where !connector.IsConnected
                       select pipeElement];

        }

        public static IEnumerable<Element> GetUnenclosedRooms(this Document doc)
        {
            new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Rooms)
                        .WhereElementIsNotElementType()
                        .Cast<Room>();

            return [.. from room in new FilteredElementCollector(doc)
                         .OfCategory(BuiltInCategory.OST_Rooms)
                         .WhereElementIsNotElementType()
                         .Cast<Room>()
                           where room is not null
                           let r = room
                           let boundary = r.GetBoundarySegments(new SpatialElementBoundaryOptions())
                       where boundary is null && boundary.Count == 0
                       select room ];
        }

        public static IEnumerable<Element> GetUnusedFamilies(this Document doc)
        {
            return new FilteredElementCollector(doc)
                           .WhereElementIsNotElementType()
                           .OfClass(typeof(Family))
                           .Cast<Family>()
                           .Where(x => !x.GetFamilySymbolIds().Any())
                           .AsEnumerable();
        }

        public static IEnumerable<Element> GetViews(this Document doc)
        {
            return [.. from view in new FilteredElementCollector(doc)
                       .WhereElementIsNotElementType()
                       .OfClass(typeof(View))
                       where view is not null
                       let viewCheck = view as View
                       where viewCheck.IsTemplate is false
                       select view];
        }

        public static IEnumerable<Element> GetViewsNotInSheets(this Document doc)
        {
            using FilteredElementCollector viewCollector = new FilteredElementCollector(doc)
                       .OfClass(typeof(View))
                       .WhereElementIsNotElementType();

            using FilteredElementCollector viewportCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(Viewport));

            HashSet<ElementId> viewsOnSheets = [.. viewportCollector.Select(vp => ((Viewport)vp).ViewId)];

            return from Element viewElement in viewCollector
                   where viewElement is View view && !view.IsTemplate && !viewsOnSheets.Contains(view.Id)
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
                                   .OfClass(typeof(Family))
                                   .Cast<Family>()
                                   .Where(x => x.IsEditable && x.IsValidObject && x is not null)
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
