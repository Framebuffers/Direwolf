using Autodesk.Private.InfoCenter;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Direwolf.Revit.Benchmarking
{
    internal static class CheckHelpers
    {

        public static string GetAnnotativeElements(Document doc)
        {
            using FilteredElementCollector collector = new(doc);
            ICollection<Element> annotativeElements = collector
                .WhereElementIsNotElementType()
                .Where(e => e.Category != null && e.Category.CategoryType == CategoryType.Annotation)
                .ToList();


            var results = new List<string>();
            foreach (Element element in annotativeElements)
            {
                results.Add(element.Id.Value.ToString());
            }

            return results.Count != 0
                ? string.Join("\n", results)
                : "None found";
        }

        public static string GetDesignOptions(Document doc)
        {
            using FilteredElementCollector collector = new(doc);
            ICollection<DesignOption> elements = collector
                            .WhereElementIsNotElementType()
                            .OfClass(typeof(DesignOption))
                            .Cast<DesignOption>()
                            .ToList();

            List<string> designOptionNames = [];
            designOptionNames.AddRange(collector.Select(element => element.Name));
            return string.Join("\n", designOptionNames);
        }

        public static string GetDetailGroups(Document doc)
        {

            using FilteredElementCollector collector = new(doc);
            ICollection<Group> detailGroups = collector
                .WhereElementIsNotElementType()
                .OfClass(typeof(Group))
                .Cast<Group>()
                .Where(e => e.Category != null && e.Category.Name == "Detail Groups")
                .ToList();

            var results = new Dictionary<string, string>();
            foreach (Element element in detailGroups)
            {
                results[element.UniqueId] = element.Id.ToString();
            }
            return string.Join("\n", results.Select(x => $"{x.Key}: {x.Value}"));
        }

        // too many false positives because of double precision.
        public static string GetDuplicateElements(Document doc)
        {
            using FilteredElementCollector collector = new(doc);
            ICollection<Element> elements = collector
                .WhereElementIsNotElementType()
                .WhereElementIsViewIndependent()
                .Cast<Element>()
                .ToList();

            Dictionary<string, List<Element>> elementGroups = [];

            foreach (Element element in elements)
            {
                try
                {
                    using Location location = element.Location;
                    switch (location)
                    {
                        case LocationPoint locationPoint:
                            {
                                string key = $"{element.GetType().Name}-{double.Round(locationPoint.Point.X)},{double.Round(locationPoint.Point.Y)},{double.Round(locationPoint.Point.Z)}";

                                if (!elementGroups.TryGetValue(key, out List<Element>? value))
                                {
                                    value = [];
                                    elementGroups[key] = value;
                                }
                                value.Add(element);
                                break;
                            }

                        case LocationCurve locationCurve:
                            {
                                string key = $"{element.GetType().Name}-{locationCurve.Curve.GetEndPoint(0)}-{locationCurve.Curve.GetEndPoint(1)}";

                                if (!elementGroups.TryGetValue(key, out List<Element>? value))
                                {
                                    value = [];
                                    elementGroups[key] = value;
                                }
                                value.Add(element);
                                break;
                            }
                    }
                }
                catch (Exception e)
                {
                    Debug.Print(e.Message);
                    continue;
                }
            }
            return string.Join("\n", elementGroups.Select(pair => $"{pair.Key}: {pair.Value}"));
        }

        //public static string GetDWGFiles(Document doc)
        //{
            
        //    //using FilteredElementCollector collector = new(doc);
        //    //ICollection<Element> linkedDWGFiles = collector
        //    //    .OfClass(typeof(ImportInstance))
        //    //    .WhereElementIsNotElementType()
        //    //    .ToList();

        //    //var dwg = doc.
        //    //    ProjectInformation.GetExternalResourceReferencesExpanded();



        //    //var results = new Dictionary<string, string>();
        //    //foreach (Element element in linkedDWGFiles)
        //    //{
        //    //    string elementName = element.Name;
        //    //    ElementId elementId = element.Id;

        //    //    Parameter pathParam = element.get_Parameter(BuiltInParameter.IMPORT_SYMBOL_NAME);
        //    //    string filePath = pathParam != null ? pathParam.AsString() : "Unknown Path";
        //    //    results.Add(element.UniqueId, filePath);
        //    //}

        //    //return string.Join("\n", results.Select(x => $"{x.Key}: {x.Value}"));
        //}

        public static string GetElementsByWorkset(Document doc)
        {

            using FilteredElementCollector collector = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType();

            Dictionary<string, int> worksetElementCount = [];

            foreach (Element element in collector)
            {
                using Parameter worksetParam = element.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);

                if (worksetParam != null)
                {
                    string worksetName = worksetParam.AsValueString();

                    if (worksetElementCount.TryGetValue(worksetName, out int value))
                    {
                        worksetElementCount[worksetName] = value++;
                    }
                    else
                    {
                        worksetElementCount[worksetName] = 1;
                    }
                }
            }

            return string.Join("\n", worksetElementCount
                .OrderByDescending(pair => pair.Value)
                .Select(pair => $"Workset: {pair.Key}, Elements: {pair.Value}"));

        }

        public static string GetErrorsAndWarnings(Document doc)
        {
            Dictionary<string, string> failures = [];
            foreach (FailureMessage failureMessage in doc.GetWarnings())
            {
                failures[failureMessage.GetDescriptionText() + " " + Guid.NewGuid()] = failureMessage.GetSeverity().ToString();
            }

            return string.Join("\n", failures.Select(x => $"{x.Key}: {x.Value}"));
        }

        public static int GetFamilyCount(Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(Family))
                .GetElementCount();
        }

        public static string GetFamilyCountBySize(Document doc)
        {
            using FilteredElementCollector familyCollector = new(doc);

            Dictionary<string, int> familyCounts = [];

            foreach (Family family in familyCollector)
            {
                ICollection<ElementId> familyTypeIds = family.GetFamilySymbolIds();
                int count = 0;

                foreach (ElementId typeId in familyTypeIds)
                {
                    count += new FilteredElementCollector(doc)
                        .OfCategoryId(family.FamilyCategory.Id)
                        .WhereElementIsNotElementType()
                        .Where(e => e.GetTypeId() == typeId)
                        .ToList()
                        .Count;
                }

                familyCounts[family.Name] = count;
            }

            var sortedFamilies = familyCounts.OrderByDescending(pair => pair.Value);
            return string.Join("\n", sortedFamilies.Select(pair => $"{pair.Key}: {pair.Value} instances"));
        }

        public static int GetGridLineCount(Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(Grid))
                .GetElementCount();
        }

        public static string GetImportedImages(Document doc)
        {
            ICollection<Element> importedImages = new FilteredElementCollector(doc)
                .OfClass(typeof(ImportInstance))
                .WhereElementIsNotElementType()
                .ToList();

            var results = importedImages.Select(element => element.Id.Value.ToString()).ToList();

            return results.Count != 0
                ? string.Join("\n", results)
                : "None found";
        }

        public static string GetInPlaceFamilies(Document doc)
        {
            using FilteredElementCollector familyInstanceCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilyInstance));

            List<string> inPlaceFamilies = [];

            foreach (Element element in familyInstanceCollector)
            {
                if (element is FamilyInstance familyInstance)
                {
                    using Family family = familyInstance.Symbol.Family;

                    if (family.IsInPlace)
                    {
                        inPlaceFamilies.Add($"In-Place Family Name: {family.Name}, ID: {family.Id}");
                    }
                }
            }

            return inPlaceFamilies.Count > 0
                ? string.Join("\n", inPlaceFamilies)
                : "None found";
        }

        public static string GetLargestFamilies(Document doc)
        {
            using FilteredElementCollector familyCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(Family));

            List<string> largeFamilies = [];

            largeFamilies.AddRange(from Family family in familyCollector
                                   let typeCount = family.GetFamilySymbolIds().Count
                                   where typeCount > 50
                                   select $"Family Name: {family.Name}, Types: {typeCount}");

            return largeFamilies.Count > 0
                ? string.Join("\n", largeFamilies)
                : "None found";
        }

        public static int GetLevelCount(Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .GetElementCount();
        }

        public static string GetMirroredObjects(Document doc)
        {
            List<FamilyInstance> familyInstances = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilyInstance))
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();

            List<string> mirroredElements = familyInstances.Where(instance => instance.GetTransform().HasReflection)
                                                           .Select(instance => instance.Id.Value.ToString())
                                                           .ToList();

            return mirroredElements.Count > 0
                ? string.Join("\n", mirroredElements)
                : "None found";
        }

        public static string GetModelGroups(Document doc)
        {

            ICollection<Element> modelGroups = new FilteredElementCollector(doc)
                .OfClass(typeof(Group))
                .WhereElementIsNotElementType()
                .ToList();

            var results = new Dictionary<string, string>();
            foreach (Element element in modelGroups)
            {
                results[element.UniqueId] = element.Id.Value.ToString();
            }

            return string.Join("\n", results.Select(x => $"{x.Key}: {x.Value}"));
        }

        public static string GetNonNativeObjectStyles(Document doc)
        {
            using FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfClass(typeof(GraphicsStyle));

            List<string> nonNativeObjectStyles = [];

            foreach (Element element in collector)
            {
                if (element is GraphicsStyle graphicsStyle)
                {
                    using Category category = graphicsStyle.GraphicsStyleCategory;
                    if (category != null && category.IsCuttable == false && category.CategoryType == CategoryType.Annotation)
                    {
                        nonNativeObjectStyles.Add($"Style Name: {graphicsStyle.Name}, Category: {category.Name}");
                    }
                }
            }

            return nonNativeObjectStyles.Count > 0
                ? string.Join("\n", nonNativeObjectStyles)
                : "No non-native object styles found.";
        }

        public static string GetSKPFiles(Document doc)
        {
            using FilteredElementCollector filteredElementCollector = new(doc);
            ICollection<Element> importedElements = filteredElementCollector
                .OfClass(typeof(ImportInstance))
                .WhereElementIsNotElementType()
                .ToList();

            var results = new Dictionary<string, string>();
            foreach (Element element in importedElements)
            {
                using Parameter pathParam = element.get_Parameter(BuiltInParameter.IMPORT_SYMBOL_NAME);
                string filePath = pathParam != null ? pathParam.AsString() : "Unknown Path";
                results[element.UniqueId] = filePath;
            }

            return string.Join("\n", results.Select(x => $"{x.Key}: {x.Value}"));
        }

        public static string GetUnconnectedDucts(Document doc)
        {
            using FilteredElementCollector ductCollector = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_DuctCurves)
                .WhereElementIsNotElementType();

            List<string> unconnectedDucts = [];

            foreach (Element ductElement in ductCollector)
            {
                if (ductElement is Duct duct)
                {
                    bool isUnconnected = false;

                    ConnectorSet connectors = duct.ConnectorManager.Connectors;
                    foreach (Connector connector in connectors)
                    {
                        if (!connector.IsConnected)
                        {
                            isUnconnected = true;
                            break;
                        }
                    }

                    if (isUnconnected)
                    {
                        unconnectedDucts.Add($"Duct Name: {duct.Name}, Duct ID: {duct.Id}");
                    }
                }
            }

            return unconnectedDucts.Count > 0
                ? string.Join("\n", unconnectedDucts)
                : "No unconnected ducts found.";
        }

        public static string GetUnconnectedElectrical(Document doc)
        {
            using FilteredElementCollector electricalCollector = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_ElectricalFixtures)
                .WhereElementIsNotElementType();

            List<string> unconnectedConnections = [];

            foreach (Element electricalElement in electricalCollector)
            {
                using MEPModel mepModel = ((FamilyInstance)electricalElement).MEPModel;
                if (mepModel != null)
                {
                    using ConnectorSet connectors = mepModel.ConnectorManager.Connectors;
                    foreach (Connector connector in connectors)
                    {
                        if (!connector.IsConnected)
                        {
                            unconnectedConnections.Add($"Element Name: {electricalElement.Name}, ID: {electricalElement.Id}, Connector ID: {connector.Id}");
                        }
                    }
                }
            }

            return unconnectedConnections.Count > 0
                ? string.Join("\n", unconnectedConnections)
                : "No unconnected found";

        }

        public static string GetUnconnectedPipes(Document doc)
        {
            using FilteredElementCollector pipeCollector = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_PipeCurves)
                .WhereElementIsNotElementType();

            List<string> unconnectedPipes = [];

            foreach (Element pipeElement in pipeCollector)
            {
                if (pipeElement is Pipe pipe)
                {
                    bool isUnconnected = false;

                    ConnectorSet connectors = pipe.ConnectorManager.Connectors;
                    foreach (Connector connector in connectors)
                    {
                        if (!connector.IsConnected)
                        {
                            isUnconnected = true;
                            break;
                        }
                    }

                    if (isUnconnected)
                    {
                        unconnectedPipes.Add($"Pipe Name: {pipe.Name}, Pipe ID: {pipe.Id}");
                    }
                }
            }

            return unconnectedPipes.Count > 0
                ? string.Join("\n", unconnectedPipes)
                : "None found";
        }

        public static string GetUnenclosedRooms(Document doc)
        {
            using FilteredElementCollector roomCollector = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType();

            List<string> unenclosedRooms = [];

            foreach (Element roomElement in roomCollector)
            {
                if (roomElement is Room room)
                {
                    IList<IList<BoundarySegment>> boundarySegments = room.GetBoundarySegments(new SpatialElementBoundaryOptions());

                    if (boundarySegments == null || boundarySegments.Count == 0)
                    {
                        unenclosedRooms.Add($"Room Name: {room.Name}, Room ID: {room.Id}");
                    }
                }
            }

            return unenclosedRooms.Count != 0
                ? string.Join("\n", unenclosedRooms)
                : "No unenclosed rooms found.";

        }

        public static string GetUnusedFamilies(Document doc)
        {
            using FilteredElementCollector familyCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(Family));

            List<string> unusedFamilies = [];

            foreach (Family family in familyCollector.Cast<Family>())
            {
                try
                {
                    using FilteredElementCollector instanceCollector = new FilteredElementCollector(doc)
                        .OfCategory(family.Category.BuiltInCategory)
                        .WhereElementIsNotElementType();

                    if (!instanceCollector.Any())
                    {
                        unusedFamilies.Add(family.Name);
                    }
                }
                catch
                {
                    continue;
                }
            }

            return unusedFamilies.Count != 0
                ? string.Join("\n", unusedFamilies)
                : "No unused families found.";
        }

        public static string GetViews(Document doc)
        {
            using FilteredElementCollector viewCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .WhereElementIsNotElementType();

            List<string> views = [];

            foreach (Element elem in viewCollector)
            {
                if (elem is View view && !view.IsTemplate)
                {
                    views.Add(view.Name);
                }
            }

            return views.Count != 0
                ? string.Join("\n", views)
                : "None found";
        }

        public static string GetViewsNotInSheets(Document doc)
        {
            using FilteredElementCollector viewCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .WhereElementIsNotElementType();

            FilteredElementCollector viewportCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(Viewport));

            HashSet<ElementId> viewsOnSheets = [.. viewportCollector.Select(vp => (vp as Viewport).ViewId)];

            List<string> viewsNotOnSheets = [];

            foreach (Element viewElement in viewCollector)
            {
                if (viewElement is View view && !view.IsTemplate && !viewsOnSheets.Contains(view.Id))
                {
                    viewsNotOnSheets.Add($"View Name: {view.Name}, View ID: {view.Id}");
                }
            }

            return viewsNotOnSheets.Count != 0
                ? string.Join("\n", viewsNotOnSheets)
                : "All views are placed on sheets.";

        }

        public static string LargestFamilies(Document doc)
        {
            using FilteredElementCollector familyCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(Family));

            Dictionary<string, int> familySizes = [];

            foreach (Family family in familyCollector)
            {
                ICollection<ElementId> familyTypeIds = family.GetFamilySymbolIds();
                int totalInstances = 0;

                foreach (ElementId typeId in familyTypeIds)
                {
                    totalInstances += new FilteredElementCollector(doc)
                        .WhereElementIsNotElementType()
                        .Where(e => e.GetTypeId() == typeId)
                        .Count();
                }

                familySizes[family.Name] = totalInstances;
            }

            var largestFamilies = familySizes
                .OrderByDescending(pair => pair.Value)
                .Take(10);

            List<string> results = largestFamilies
                .Select(pair => $"Family Name: {pair.Key}, Instances: {pair.Value}")
                .ToList();

            return results.Count != 0
                ? string.Join("\n", results)
                : "No families found.";
        }

        public static string TotalSizeOfFamiliesByMB(Document doc)
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            Directory.CreateDirectory(Path.Combine(desktop, "rvt"));

            string folderPath = Path.GetFullPath(Path.Combine(desktop, "rvt"));
            double totalSizeInMB = 0;

            SortedDictionary<long, string> sorted = [];

            if (Directory.Exists(folderPath))
            {
                using FilteredElementCollector familyCollector = new FilteredElementCollector(doc)
                    .OfClass(typeof(Family));

                foreach (Family family in familyCollector.Cast<Family>())
                {
                    string familyPath = Path.Combine(folderPath, $"{family.Name}.rfa");
                    if (family.IsEditable)
                    {
                        if (!File.Exists(familyPath))
                        {
                            doc.EditFamily(family).SaveAs(familyPath);
                            long length = new FileInfo(familyPath).Length / (1024);
                            if (!sorted.TryGetValue(length, out string familyName))
                                sorted.Add(length, familyName);
                        }
                        else
                        {
                            totalSizeInMB = new DirectoryInfo(folderPath).EnumerateFiles()
                                .OrderByDescending(x => x.Length)
                                .FirstOrDefault()
                                .Length / (1024);
                        }
                    }
                }

                if (sorted.Count != 0) totalSizeInMB = sorted.FirstOrDefault().Key;


                return $"Total Size of All Families: {totalSizeInMB} KB";
            }
            else
            {
                return "The specified folder does not exist.";
            }
        }
    }
}