using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Direwolf.Definitions;
using Direwolf.Revit.Definitions;
using Direwolf.Revit.Utilities;
using System.Diagnostics;
using System.Net;
using System.Linq;

namespace Direwolf.Revit.ElementFilters
{
    public static class DocumentExtensions
    {
        public static Prey GetAnnotativeElements(this Document doc)
        {

            IEnumerable<ElementInformation> info()
            {
                using FilteredElementCollector collector = new(doc);
                ICollection<Element> annotativeElements = [.. collector
                    .WhereElementIsNotElementType()
                    .Where(e => e.Category != null
                           && e.Category.CategoryType == CategoryType.Annotation)];

                foreach (var e in annotativeElements)
                {
                    yield return e.GetElementInformation(doc);
                }
            }

            return new Prey(info());
        }

        public static Prey GetDesignOptions(this Document doc)
        {
            IEnumerable<ElementInformation> info()
            {
                using FilteredElementCollector collector = new(doc);
                ICollection<DesignOption> elements = collector
                                .WhereElementIsNotElementType()
                                .OfClass(typeof(DesignOption))
                                .Cast<DesignOption>()
                                .ToList();

                foreach (var e in elements)
                {
                    yield return e.GetElementInformation(doc);
                }
            }

            return new Prey(info());
        }

        public static Prey GetDetailGroups(this Document doc)
        {
            IEnumerable<ElementInformation> info()
            {
                using FilteredElementCollector collector = new(doc);
                ICollection<Group> elements = collector
                    .WhereElementIsNotElementType()
                    .OfClass(typeof(Group))
                    .Cast<Group>()
                    .Where(e => e.Category != null && e.Category.Name == "Detail Groups")
                    .ToList();
                foreach (var e in elements)
                {
                    yield return e.GetElementInformation(doc);
                }
            }
            return new Prey(info());
        }

        // too many false positives because of double precision.
        //public static Prey GetDuplicateElements(this Document doc)
        //{

        //    IEnumerable<ElementInformation> info()
        //    {
        //        using FilteredElementCollector collector = new(doc);
        //        ICollection<Element> elements = collector
        //            .WhereElementIsNotElementType()
        //            .WhereElementIsViewIndependent()
        //            .Cast<Element>()
        //            .ToList();

        //        Dictionary<string, List<Element>> elementGroups = [];

        //        foreach (Element e in elements)
        //        {
        //            try
        //            {
        //                using Location location = e.Location;
        //                switch (location)
        //                {
        //                    case LocationPoint locationPoint:
        //                        {
        //                            string key = $"{e.GetType().Name}-{double.Round(locationPoint.Point.X)},{double.Round(locationPoint.Point.Y)},{double.Round(locationPoint.Point.Z)}";

        //                            if (!elementGroups.TryGetValue(key, out List<Element>? value))
        //                            {
        //                                value = [];
        //                                elementGroups[key] = value;
        //                            }
        //                            value.Add(e);
        //                            break;
        //                        }

        //                    case LocationCurve locationCurve:
        //                        {
        //                            string key = $"{e.GetType().Name}-{locationCurve.Curve.GetEndPoint(0)}-{locationCurve.Curve.GetEndPoint(1)}";

        //                            if (!elementGroups.TryGetValue(key, out List<Element>? value))
        //                            {
        //                                value = [];
        //                                elementGroups[key] = value;
        //                            }
        //                            value.Add(e);
        //                            break;
        //                        }

        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                Debug.Print(e.Message);
        //                continue;
        //            }
        //        }
        //        foreach (var e in elementGroups)
        //        {
        //            yield return e.GetElementInformation(doc);
        //        }

        //    }
        //    return new Prey(info());
        //}

        public static Dictionary<string, int> GetElementsByWorkset(this Document doc)
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
            return worksetElementCount;
        }

        public static Prey GetErrorsAndWarnings(this Document doc)
        {
            Dictionary<string, string> failures = [];
            foreach (FailureMessage elements in doc.GetWarnings())
            {
                failures[elements.GetDescriptionText() + " " + Guid.NewGuid()] = elements.GetSeverity().ToString();
            }
            return new Prey(new Prey(failures));
        }

        public static int GetFamilyCount(this Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(Family))
                .GetElementCount();
        }

        public static Prey GetFamiliesWithMoreInstances(this Document doc)
        {
            using FilteredElementCollector familyCollector = new(doc);
            Dictionary<string, int> familyCounts = [];

            foreach (Family family in familyCollector.Cast<Family>())
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
            return new Prey(familyCounts);
        }

        public static int GetGridLineCount(this Document doc)
        {
            return new FilteredElementCollector(doc)
                 .OfClass(typeof(Grid))
                 .GetElementCount();
        }

        public static Prey GetImportedImages(this Document doc)
        {
            IEnumerable<ElementInformation> info()
            {
                ICollection<Element> importedImages = new FilteredElementCollector(doc)
                    .OfClass(typeof(ImportInstance))
                    .WhereElementIsNotElementType()
                    .ToList();
                foreach (var img in importedImages)
                {
                    yield return img.GetElementInformation(doc);
                }
            }
            return new Prey(info());
        }

        public static Prey GetInPlaceFamilies(this Document doc)
        {
            IEnumerable<ElementInformation> info()
            {
                using FilteredElementCollector familyInstanceCollector = new FilteredElementCollector(doc)
                              .OfClass(typeof(FamilyInstance));

                foreach (Element element in familyInstanceCollector)
                {
                    if (element is FamilyInstance familyInstance)
                    {
                        using Family family = familyInstance.Symbol.Family;

                        if (family.IsInPlace)
                        {
                            yield return element.GetElementInformation(doc);
                        }
                    }
                }
            }
            return new Prey(info());
        }

        public static Prey GetFamliesWithMostTypes(this Document doc)
        {
            IEnumerable<ElementInformation> info()
            {
                using FilteredElementCollector familyCollector = new FilteredElementCollector(doc)
                          .OfClass(typeof(Family));

                List<Element> largeFamilies = [];
                largeFamilies.AddRange(from Family family in familyCollector
                                       let typeCount = family.GetFamilySymbolIds().Count
                                       where typeCount > 50
                                       select family);

                foreach (var e in largeFamilies)
                {
                    yield return e.GetElementInformation(doc);
                }
            }
            return new Prey(info());
        }

        public static int GetLevelCount(this Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .GetElementCount();
        }

        public static Prey GetMirroredObjects(this Document doc)
        {
            IEnumerable<ElementInformation> info()
            {
                List<FamilyInstance> familyInstances = new FilteredElementCollector(doc)
                             .OfClass(typeof(FamilyInstance))
                             .WhereElementIsNotElementType()
                             .Cast<FamilyInstance>()
                             .ToList();
                return familyInstances.Where(e => e.GetTransform().HasReflection).Select(e => e.GetElementInformation(doc));
            }
            return new Prey(info());
        }

        public static Prey GetModelGroups(this Document doc)
        {
            IEnumerable<ElementInformation> info()
            {
                ICollection<Element> modelGroups = new FilteredElementCollector(doc)
                              .OfClass(typeof(Group))
                              .WhereElementIsNotElementType()
                              .ToList();
                return modelGroups.Select(element => element.GetElementInformation(doc));
            }
            return new Prey(info());
        }

        public static Prey GetNonNativeObjectStyles(this Document doc)
        {
            IEnumerable<ElementInformation> info()
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
                            yield return element.GetElementInformation(doc);
                        }
                    }
                }
            }
            return new Prey(info());
        }

        public static Prey GetSKPFiles(this Document doc)
        {
            IEnumerable<ElementInformation> info()
            {
                using FilteredElementCollector filteredElementCollector = new(doc);
                ICollection<Element> importedElements = filteredElementCollector
                    .OfClass(typeof(ImportInstance))
                    .WhereElementIsNotElementType()
                    .ToList();

                var results = new Dictionary<string, string>();
                foreach (Element element in importedElements)
                {
                    yield return element.GetElementInformation(doc);
                }

            }

            return new Prey(info());
        }

        public static Prey GetUnconnectedDucts(this Document doc)
        {
            IEnumerable<ElementInformation> info()
            {

                using FilteredElementCollector ductCollector = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_DuctCurves)
                    .WhereElementIsNotElementType();

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
                            yield return ductElement.GetElementInformation(doc);
                        }
                    }
                }
            }
            return new Prey(info());
        }

        public static Prey GetUnconnectedElectrical(this Document doc)
        {
            IEnumerable<ElementInformation> info()
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
                                yield return electricalElement.GetElementInformation(doc);
                            }
                        }
                    }
                }
            }
            return new Prey(info());
        }

        public static Prey GetUnconnectedPipes(this Document doc)
        {
            IEnumerable<ElementInformation> info()
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
                            yield return pipeElement.GetElementInformation(doc);
                        }
                    }
                }
            }
            return new Prey(info());
        }

        public static Prey GetUnenclosedRooms(this Document doc)
        {
            IEnumerable<ElementInformation> info()
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
                            yield return roomElement.GetElementInformation(doc);
                        }
                    }
                }
            }
            return new Prey(info());
        }

        public static Prey GetUnusedFamilies(this Document doc)
        {
            IEnumerable<ElementInformation> info()
            {

                using FilteredElementCollector familyCollector = new FilteredElementCollector(doc)
                    .OfClass(typeof(Family));

                List<ElementInformation> unusedFamilies = [];

                foreach (Family family in familyCollector.Cast<Family>())
                {
                    try
                    {
                        using FilteredElementCollector instanceCollector = new FilteredElementCollector(doc)
                            .OfCategory(family.Category.BuiltInCategory)
                            .WhereElementIsNotElementType();

                        if (!instanceCollector.Any())
                        {
                            unusedFamilies.Add(family.GetElementInformation(doc));
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                foreach (var family in unusedFamilies) { yield return family; }
            }
            return new Prey(info());
        }

        public static Prey GetViews(this Document doc)
        {
            IEnumerable<ElementInformation> info()
            {

            }
            return new Prey(info());



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
        }

        public static Prey GetViewsNotInSheets(this Document doc)
        {
            IEnumerable<ElementInformation> info()
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
            }

            return new Prey(info());
        }

        public static Prey LargestFamilies(this Document doc)
        {
            IEnumerable<ElementInformation> info()
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

            }
            return new Prey(info());
        }

        public static Prey TotalSizeOfFamiliesByMB(this Document doc)
        {
            IEnumerable<ElementInformation> info()
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
                                long length = new FileInfo(familyPath).Length / 1024;
                                if (!sorted.TryGetValue(length, out string familyName))
                                    sorted.Add(length, familyName);
                            }
                            else
                            {
                                totalSizeInMB = new DirectoryInfo(folderPath).EnumerateFiles()
                                    .OrderByDescending(x => x.Length)
                                    .FirstOrDefault()
                                    .Length / 1024;
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
            return new Prey(info());
        }

    }
}
