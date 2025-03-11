using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Direwolf.Definitions;
using Direwolf.Revit.Howls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Direwolf.Revit.Benchmarking
{
    public record class GetRevitModelHealth : RevitHowl
    {
    }

    [Transaction(TransactionMode.Manual)]
    public class BaseCommandSanityCheck : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            using StringWriter s = new();
            string lastCommand = string.Empty;

            try
            {
                s.WriteLine(GetViews(doc));
                lastCommand = "GetViews";
                Debug.Print(s.ToString());

                s.WriteLine(GetAnnotativeElements(doc));
                lastCommand = "GetAnnotativeElements";
                Debug.Print(s.ToString());

                s.WriteLine(GetImportedImages(doc));
                lastCommand = "GetImportedImages";
                Debug.Print(s.ToString());

                s.WriteLine(GetDWGFiles(doc));
                lastCommand = "GetDWGFiles";
                Debug.Print(s.ToString());

                s.WriteLine(GetSKPFiles(doc));
                lastCommand = "GetSKPFiles";
                Debug.Print(s.ToString());

                s.WriteLine(GetModelGroups(doc));
                lastCommand = "GetModelGroups";
                Debug.Print(s.ToString());

                s.WriteLine(GetDetailGroups(doc));
                lastCommand = "GetDetailGroups";
                Debug.Print(s.ToString());

                s.WriteLine(GetDesignOptions(doc));
                lastCommand = "GetDesignOptions";
                Debug.Print(s.ToString());

                s.WriteLine(GetLevelCount(doc));
                lastCommand = "GetLevelCount";
                Debug.Print(s.ToString());

                s.WriteLine(GetGridLineCount(doc));
                lastCommand = "GetGridLineCount";
                Debug.Print(s.ToString());

                s.WriteLine(GetErrorsAndWarnings(doc));
                lastCommand = "GetErrorsAndWarnings";
                Debug.Print(s.ToString());

                s.WriteLine(GetUnusedFamilies(doc));
                lastCommand = "GetUnusedFamilies";

                s.WriteLine(GetUnenclosedRooms(doc));
                lastCommand = "GetUnenclosedRooms";
                Debug.Print(s.ToString());

                s.WriteLine(GetDuplicateElements(doc));
                lastCommand = "GetDuplicateElements";
                Debug.Print(s.ToString());

                s.WriteLine(GetViewsNotInSheets(doc));
                lastCommand = "GetViewsNotInSheets";
                Debug.Print(s.ToString());

                s.WriteLine(GetUnconnectedDucts(doc));
                lastCommand = "GetUnconnectedDucts";
                Debug.Print(s.ToString());

                s.WriteLine(GetUnconnectedPipes(doc));
                lastCommand = "GetUnconnectedPipes";
                Debug.Print(s.ToString());

                s.WriteLine(GetUnconnectedElectrical(doc));
                lastCommand = "GetUnconnectedElectrical";
                Debug.Print(s.ToString());

                s.WriteLine(GetNonNativeObjectStyles(doc));
                lastCommand = "GetNonNativeObjectStyles";
                Debug.Print(s.ToString());

                s.WriteLine(GetMirroredObjects(doc));
                lastCommand = "GetMirroredObjects";
                Debug.Print(s.ToString());

                s.WriteLine(GetElementsByWorkset(doc));
                lastCommand = "GetElementsByWorkset";
                Debug.Print(s.ToString());

                s.WriteLine(GetFamilyCount(doc));
                lastCommand = "GetFamilyCount";
                s.WriteLine(lastCommand);
                Debug.Print(s.ToString());

                s.WriteLine(GetFamilyCountBySize(doc));
                lastCommand = "GetFamilyCountBySize";
                s.WriteLine(lastCommand);
                Debug.Print(s.ToString());

                s.WriteLine(GetLargestFamilies(doc));
                lastCommand = "GetLargestFamilies";
                s.WriteLine(lastCommand);
                Debug.Print(s.ToString());

                s.WriteLine(GetInPlaceFamilies(doc));
                lastCommand = "GetInPlaceFamilies";
                s.WriteLine(lastCommand);
                Debug.Print(s.ToString());

                s.WriteLine(TotalSizeOfFamiliesByMB(doc));
                lastCommand = "TotalSizeByMB";
                s.WriteLine(lastCommand);
                Debug.Print(s.ToString());

                s.WriteLine(GetElementsByWorkset(doc));
                lastCommand = "Elements by Workset";
                s.WriteLine(lastCommand);
                Debug.Print(s.ToString());

            }
            catch
            {
                Debug.Print(lastCommand);
            }

            Debug.Print(s.ToString());
            return Result.Succeeded;
        }

        private string GetViews(Document doc)
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .WhereElementIsNotElementType();

            List<string> views = [];

            foreach (Element elem in viewCollector)
            {
                View view = elem as View;
                if (view != null && !view.IsTemplate)
                {
                    views.Add(view.Name);
                }
            }

            return views.Count != 0
                ? string.Join("\n", views)
                : "None found";
        }

        private string GetAnnotativeElements(Document doc)
        {
            FilteredElementCollector collector = new(doc);

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

        private string GetImportedImages(Document doc)
        {
            FilteredElementCollector collector = new(doc);

            ICollection<Element> importedImages = collector
                .OfClass(typeof(ImportInstance)) 
                .WhereElementIsNotElementType()
                .ToList();

            var results = new List<string>();
            foreach (Element element in importedImages)
            {
                results.Add(element.Id.Value.ToString());
            }

            return results.Count != 0
                ? string.Join("\n", results)
                : "None found";
        }

        private string GetDWGFiles(Document doc)
        {
            FilteredElementCollector collector = new(doc);

            ICollection<Element> linkedDWGFiles = collector
                .OfClass(typeof(ImportInstance))
                .WhereElementIsNotElementType()
                .ToList();

            var results = new Dictionary<string, string>();
            foreach (Element element in linkedDWGFiles)
            {
                string elementName = element.Name;
                ElementId elementId = element.Id;

                Parameter pathParam = element.get_Parameter(BuiltInParameter.IMPORT_SYMBOL_NAME);
                string filePath = pathParam != null ? pathParam.AsString() : "Unknown Path";
                results.Add(element.UniqueId, filePath);
            }

            return string.Join("\n", results.Select(x => $"{x.Key}: {x.Value}"));
        }

        private string GetSKPFiles(Document doc)
        {
            FilteredElementCollector collector = new(doc);

            ICollection<Element> importedElements = collector
                .OfClass(typeof(ImportInstance)) 
                .WhereElementIsNotElementType()
                .ToList();

            var results = new Dictionary<string, string>();
            foreach (Element element in importedElements)
            {
                Parameter pathParam = element.get_Parameter(BuiltInParameter.IMPORT_SYMBOL_NAME);
                string filePath = pathParam != null ? pathParam.AsString() : "Unknown Path";

                results[element.UniqueId] = filePath;
            }
            return string.Join("\n", results.Select(x => $"{x.Key}: {x.Value}"));
        }

        private string GetModelGroups(Document doc)
        {
            FilteredElementCollector collector = new(doc);

            ICollection<Element> modelGroups = collector
                .OfClass(typeof(Group)) 
                .WhereElementIsNotElementType()
                .ToList();

            var results = new Dictionary<string, string>();
            foreach (Element element in modelGroups)
            {
                string groupName = element.Name;
                ElementId groupId = element.Id;

                results[element.UniqueId] = element.Id.Value.ToString();
            }
            return string.Join("\n", results.Select(x => $"{x.Key}: {x.Value}"));

        }

        private string GetDetailGroups(Document doc)
        {
            FilteredElementCollector collector = new(doc);

            ICollection<Element> detailGroups = collector
                .OfClass(typeof(Group)) 
                .WhereElementIsNotElementType() 
                .Where(e => e.Category != null && e.Category.Name == "Detail Groups") 
                .ToList();
            var results = new Dictionary<string, string>();
            foreach (Element element in detailGroups)
            {
                results[element.UniqueId] = element.Id.ToString();
            }
            return string.Join("\n", results.Select(x => $"{x.Key}: {x.Value}"));
        }

        private string GetDesignOptions(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfClass(typeof(DesignOption)) 
                .WhereElementIsNotElementType(); 

            List<string> designOptionNames = [];
            foreach (Element element in collector)
            {
                DesignOption designOption = element as DesignOption;
                if (designOption != null)
                {
                    designOptionNames.Add(designOption.Name);
                }
            }

            return string.Join("\n", designOptionNames);
        }

        private string GetLevelCount(Document doc)
        {
            FilteredElementCollector levelCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(Level)); 

            return levelCollector.Count().ToString();

        }

        private string GetGridLineCount(Document doc)
        {
            FilteredElementCollector gridCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(Grid)); 

            return gridCollector.Count().ToString();

        }

        private string GetErrorsAndWarnings(Document doc)
        {
            IList<FailureMessage> failureMessages = doc.GetWarnings();
            Dictionary<string, string> failures = [];
            foreach (FailureMessage failureMessage in failureMessages)
            {
                failures[failureMessage.GetDescriptionText() + " " + Guid.NewGuid()] = failureMessage.GetSeverity().ToString();

            }

            return string.Join("\n", failures.Select(x => $"{x.Key}: {x.Value}"));
        }

        private string GetUnusedFamilies(Document doc)
        {
            FilteredElementCollector familyCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(Family));

            List<string> unusedFamilies = [];

            foreach (Family family in familyCollector.Cast<Family>())
            {
                try
                {
                    FilteredElementCollector instanceCollector = new FilteredElementCollector(doc)
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

        private string GetUnenclosedRooms(Document doc)
        {
            FilteredElementCollector roomCollector = new FilteredElementCollector(doc)
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

        private string GetDuplicateElements(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .WhereElementIsViewIndependent();

            Dictionary<string, List<Element>> elementGroups = [];

            foreach (Element element in collector)
            {
                try
                {
                    Location location = element.Location;

                    if (location is LocationPoint locationPoint)
                    {
                        string key = $"{element.GetType().Name}-{double.Round(locationPoint.Point.X)},{double.Round(locationPoint.Point.Y)},{double.Round(locationPoint.Point.Z)}";

                        if (!elementGroups.ContainsKey(key))
                        {
                            elementGroups[key] = [];
                        }
                        elementGroups[key].Add(element);
                    }
                    else if (location is LocationCurve locationCurve)
                    {
                        string key = $"{element.GetType().Name}-{locationCurve.Curve.GetEndPoint(0)}-{locationCurve.Curve.GetEndPoint(1)}";

                        if (!elementGroups.ContainsKey(key))
                        {
                            elementGroups[key] = [];
                        }
                        elementGroups[key].Add(element);
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

        private string GetViewsNotInSheets(Document doc)
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(View)) 
                .WhereElementIsNotElementType(); 

            FilteredElementCollector viewportCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(Viewport)); 

            HashSet<ElementId> viewsOnSheets = [.. viewportCollector.Select(vp => (vp as Viewport).ViewId)];

            List<string> viewsNotOnSheets = [];

            foreach (Element viewElement in viewCollector)
            {
                View view = viewElement as View;
                if (view != null && !view.IsTemplate && !viewsOnSheets.Contains(view.Id))
                {
                    viewsNotOnSheets.Add($"View Name: {view.Name}, View ID: {view.Id}");
                }
            }

            return viewsNotOnSheets.Count != 0
                ? string.Join("\n", viewsNotOnSheets)
                : "All views are placed on sheets.";

        }

        private string GetUnconnectedDucts(Document doc)
        {
            FilteredElementCollector ductCollector = new FilteredElementCollector(doc)
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

        private string GetUnconnectedPipes(Document doc)
        {
            FilteredElementCollector pipeCollector = new FilteredElementCollector(doc)
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

        private string GetUnconnectedElectrical(Document doc)
        {
            FilteredElementCollector electricalCollector = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_ElectricalFixtures) 
                .WhereElementIsNotElementType();                   

            List<string> unconnectedConnections = [];

            foreach (Element electricalElement in electricalCollector)
            {
                MEPModel mepModel = ((FamilyInstance)electricalElement).MEPModel;
                if (mepModel != null)
                {
                    ConnectorSet connectors = mepModel.ConnectorManager.Connectors;
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

        private string GetNonNativeObjectStyles(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfClass(typeof(GraphicsStyle));

            List<string> nonNativeObjectStyles = [];

            foreach (Element element in collector)
            {
                GraphicsStyle graphicsStyle = element as GraphicsStyle;

                if (graphicsStyle != null)
                {
                    Category category = graphicsStyle.GraphicsStyleCategory;

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

        private string GetMirroredObjects(Document doc)
        {
            List<FamilyInstance> familyInstances = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilyInstance))
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList(); // Exclude element types

            List<string> mirroredElements = familyInstances.Where(instance => instance.GetTransform().HasReflection)
                                                           .Select(instance => instance.Id.Value.ToString()).ToList();

            return mirroredElements.Count > 0
                ? string.Join("\n", mirroredElements)
                : "None found";
        }

        private string GetFamilyCountBySize(Document doc)
        {
            FilteredElementCollector familyCollector = new(doc);

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

        private string GetLargestFamilies(Document doc)
        {
            FilteredElementCollector familyCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(Family)); 

            List<string> largeFamilies = [];

            foreach (Family family in familyCollector)
            {
                int typeCount = family.GetFamilySymbolIds().Count;

                if (typeCount > 50) 
                {
                    largeFamilies.Add($"Family Name: {family.Name}, Types: {typeCount}");
                }
            }

            return largeFamilies.Count > 0
                ? string.Join("\n", largeFamilies)
                : "None found";
        }

        private string GetInPlaceFamilies(Document doc)
        {
            FilteredElementCollector familyInstanceCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilyInstance)); 
            List<string> inPlaceFamilies = [];

            foreach (Element element in familyInstanceCollector)
            {
                FamilyInstance familyInstance = element as FamilyInstance;

                if (familyInstance != null)
                {
                    Family family = familyInstance.Symbol.Family;

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

        private string GetFamilyCount(Document doc)
        {
            FilteredElementCollector familyCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(Family));

            int totalFamilyCount = familyCollector.GetElementCount();

            return totalFamilyCount.ToString();
        }

        private string TotalSizeOfFamiliesByMB(Document doc)
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            Directory.CreateDirectory(Path.Combine(desktop, "rvt"));

            string folderPath = Path.GetFullPath(Path.Combine(desktop, "rvt")); 
            double totalSizeInMB = 0;

            SortedDictionary<long, string> sorted = [];

            if (Directory.Exists(folderPath))
            {
                FilteredElementCollector familyCollector = new FilteredElementCollector(doc)
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

        private string LargestFamilies(Document doc)
        {
            FilteredElementCollector familyCollector = new FilteredElementCollector(doc)
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

        private string GetElementsByWorkset(Document doc)
        {

            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType(); 

            Dictionary<string, int> worksetElementCount = [];

            foreach (Element element in collector)
            {
                Parameter worksetParam = element.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);

                if (worksetParam != null)
                {
                    string worksetName = worksetParam.AsValueString();

                    if (worksetElementCount.ContainsKey(worksetName))
                    {
                        worksetElementCount[worksetName]++;
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
    }

}
