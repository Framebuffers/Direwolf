using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Direwolf.Definitions;
using Direwolf.Revit.Utilities;
using ElementType = Autodesk.Revit.DB.ElementType;

namespace Direwolf.Revit.Howls
{
    public record class GetModelSnapshot : RevitHowl
    {
        public GetModelSnapshot(Document doc) => SetRevitDocument(doc);
        // These are all categories for which information has to be extracted.
        private List<View> viewsInsideDocument = [];
        private List<View> notInSheets = [];
        private List<Element> annotativeElements = [];
        private List<(ExternalFileReferenceType, ExternalFileReference)> externalRefs = [];
        private List<Group> modelGroups = [];
        private List<Group> detailGroups = [];
        private List<DesignOption> designOptions = [];
        private List<Level> levels = [];
        private List<Grid> grids = [];
        private List<FailureMessage> warns = [];
        private List<Room> unenclosedRoom = [];
        private List<Viewport> viewports = [];
        private List<Duct> unconnectedDucts = [];
        private List<Pipe> unconnectedPipes = [];
        private List<Connector> unconnectedElectrical = [];
        private List<GraphicsStyle> nonNativeStyles = [];
        private List<Element> isFlipped = [];
        private Dictionary<string, int> worksetElementCount = [];
        private Stack<Dictionary<string, object>> individualElementInfo = [];

        private void ProcessInfo()
        {
            var doc = GetRevitDocument();
            warns.AddRange(GetRevitDocument().GetWarnings());
            ICollection<Element> collector = [.. new FilteredElementCollector(GetRevitDocument()).WhereElementIsNotElementType()];

            foreach (Element element in collector)
            {
                var e = doc.GetElement(element.Id);
                if (e is not null && e.IsValidObject && e.Category is not null && e.Category.CategoryType is not CategoryType.Invalid || e?.Category?.CategoryType is not CategoryType.Internal)
                {
                    Parameter? worksetParam = e?.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);

                    string? familyName = string.Empty;
                    string? category = string.Empty;
                    string? builtInCategory = string.Empty;
                    string? workset = string.Empty;
                    string[]? views = [];
                    string? designOption = string.Empty;
                    string? docOwner = string.Empty;
                    string? ownerViewId = string.Empty;
                    string? worksetId = string.Empty;
                    string? createdPhaseId = string.Empty;
                    string? demolishedPhaseId = string.Empty;
                    string? groupId = string.Empty;
                    string? workshareId = string.Empty;
                    string levelId = string.Empty;
                    bool? isGrouped = false;
                    bool? isModifiable = false;
                    bool? isViewSpecific = false;
                    bool? isBuiltInCategory = false;
                    bool? isAnnotative = false;
                    bool? isModel = false;
                    bool? isPinned = false;
                    bool? isWorkshared = false;

                    FamilyInstance? fm = e as FamilyInstance;

                    // isGrouped
                    if (e?.GroupId is not null)
                    {
                        isGrouped = true;
                        groupId = e.GroupId.ToString();
                    }

                    // isModifiable
                    if (e.IsModifiable) isModifiable = true;

                    // isViewSpecific
                    if (!e.ViewSpecific)
                    {
                        familyName = fm?.Symbol.Family.Name;
                    }
                    else
                    {
                        isViewSpecific = true;
                        ownerViewId = e.OwnerViewId.ToString();
                    }

                    // isBuiltInCategory + builtInCategory
                    if (e.Category is not null && e.Category.BuiltInCategory is not BuiltInCategory.INVALID)
                    {
                        isBuiltInCategory = true;
                        category = e.Category.Name;
                    }

                    if (e.WorksetId is not null) worksetId = e.WorksetId.ToString();

                    if (e.HasPhases())
                    {
                        if (e.CreatedPhaseId is not null) createdPhaseId = e.CreatedPhaseId.ToString();
                        if (e.DemolishedPhaseId is not null) demolishedPhaseId = e.DemolishedPhaseId.ToString();
                    }

                    if (e.DesignOption is not null) designOption = e.DesignOption.Name;

                    if (e.Document is not null) docOwner = doc.CreationGUID.ToString();

                    isPinned = e.Pinned;

                    if (doc.IsWorkshared)
                    {
                        isWorkshared = true;
                        workshareId = doc.WorksharingCentralGUID.ToString();
                    }

                    if (worksetParam != null)
                    {
                        string worksetName = worksetParam.AsValueString();

                        if (worksetElementCount.TryGetValue(worksetName, out int value))
                        {
                            worksetElementCount[worksetName] = ++value;
                        }
                        else
                        {
                            worksetElementCount[worksetName] = 1;
                        }
                    }

                    if (e.LevelId is not null) levelId = e.LevelId.ToString();

                    switch (e)
                    {
                        case View:
                            View? v = e as View;

                            if (v is not null && !v.IsTemplate)
                            {
                                viewsInsideDocument.Add(v);
                            }
                            builtInCategory = e?.Category?.BuiltInCategory.ToString();
                            break;
                        case Viewport:
                            Viewport? vp = e as Viewport;
                            if (vp is not null) viewports.Add(vp);
                            builtInCategory = e?.Category?.BuiltInCategory.ToString();
                            break;
                        case Group:
                            Group? g = e as Group;
                            if (g is not null)
                            {
                                if (g.Category.Name == "Detail Groups")
                                {
                                    detailGroups.Add(g);
                                }
                                else
                                {
                                    modelGroups.Add(g);
                                }
                            }
                            builtInCategory = e?.Category?.BuiltInCategory.ToString();
                            break;
                        case DesignOption:
                            DesignOption? option = e as DesignOption;
                            if (option is not null) designOptions.Add(option);
                            builtInCategory = e?.Category?.BuiltInCategory.ToString();
                            break;
                        case Level:
                            Level? l = e as Level;
                            if (l is not null) levels.Add(l);
                            builtInCategory = e?.Category?.BuiltInCategory.ToString();
                            break;
                        case Grid:
                            Grid? gr = e as Grid;
                            if (gr is not null) grids.Add(gr);
                            builtInCategory = e?.Category?.BuiltInCategory.ToString();
                            break;
                        case GraphicsStyle:
                            GraphicsStyle? graphicsStyle = e as GraphicsStyle;
                            if (graphicsStyle is not null)
                            {
                                Category c = graphicsStyle.GraphicsStyleCategory;
                                builtInCategory = e?.Category?.BuiltInCategory.ToString();

                                if (c is not null && c.IsCuttable is not false && c.CategoryType == CategoryType.Annotation)
                                {
                                    nonNativeStyles.Add(graphicsStyle);
                                }
                            }
                            break;
                        case FamilyInstance:
                            if (fm is not null)
                            {
                                if (fm.Mirrored) isFlipped.Add(e);
                                builtInCategory = e?.Category?.BuiltInCategory.ToString();
                            }
                            break;
                        default:
                            switch (e?.Category?.BuiltInCategory)
                            {
                                case BuiltInCategory.OST_Rooms:
                                    Room? room = e as Room;
                                    if (room is not null)
                                    {
                                        IList<IList<BoundarySegment>> boundarySegments = room.GetBoundarySegments(new SpatialElementBoundaryOptions());

                                        if (boundarySegments == null || boundarySegments.Count == 0)
                                        {
                                            unenclosedRoom.Add(room);
                                        }
                                    }
                                    builtInCategory = e?.Category?.BuiltInCategory.ToString();

                                    break;
                                case BuiltInCategory.OST_DuctCurves:
                                    if (e is Duct duct)
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
                                            unconnectedDucts.Add(duct);
                                        }
                                    }
                                    builtInCategory = e?.Category?.BuiltInCategory.ToString();
                                    break;
                                case BuiltInCategory.OST_PipeCurves:
                                    if (e is Pipe pipe)
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
                                            unconnectedPipes.Add(pipe);
                                        }
                                    }
                                    builtInCategory = e?.Category?.BuiltInCategory.ToString();
                                    break;
                                case BuiltInCategory.OST_ElectricalFixtures:
                                    MEPModel mepModel = ((FamilyInstance)e).MEPModel;
                                    if (mepModel != null)
                                    {
                                        ConnectorSet connectors = mepModel.ConnectorManager.Connectors;
                                        foreach (Connector connector in connectors)
                                        {
                                            if (!connector.IsConnected)
                                            {
                                                unconnectedElectrical.Add(connector);
                                            }
                                        }
                                    }
                                    builtInCategory = e?.Category?.BuiltInCategory.ToString();
                                    break;
                                default:
                                    builtInCategory = e?.Category?.BuiltInCategory.ToString();
                                    break;
                            }

                            //TODO: check for unused
                            //Debug.Print(e.Name + "" + typeof(Element).Name);
                            break;
                    }

                    switch (e?.Category?.CategoryType)
                    {
                        case CategoryType.Model:
                            isAnnotative = false;
                            isModel = true;
                            builtInCategory = CategoryType.Model.ToString();
                            break;
                        case CategoryType.Annotation:
                            isAnnotative = true;
                            isModel = false;
                            builtInCategory = CategoryType.Annotation.ToString();
                            annotativeElements.Add(e);
                            break;
                        case CategoryType.Invalid:
                        case CategoryType.Internal:
                        case CategoryType.AnalyticalModel:
                        default:
                            break;
                    }

                    /**
                     * This code can serialize the entire model to a JSON file. However, it will export a several-hundred MB JSON file. It *will* crash the database.
                     * 
                     * In case you *need* to track every single Element, you'd need to implement a different database solution.
                     * 
                    Dictionary<string, object>? elementInfo = new()
                    {
                        ["id"] = e.Id.Value,
                        ["uniqueId"] = e.UniqueId,
                        ["familyName"] = GetElementType(e)?.FamilyName ?? string.Empty,
                        ["elementName"] = e?.Name ?? string.Empty,
                    };

                    List<Dictionary<string, object>> result =
                    [
                        elementInfo,
                                new Dictionary<string, object>() { ["parameters"] = GetParameters(e) },
                                GetCategory(e),
                                new Dictionary<string, object>() { ["materials"] = e.GetMaterialIds(true)},
                            ];

                    SendCatchToCallback(new Prey(result));
                    */


                }
            }
            // view not in sheet. needs to be done after all are done.
            HashSet<ElementId> viewsOnSheets = [.. viewports.Select(vp => (vp as Viewport).ViewId)];
            foreach (Element viewElement in viewsInsideDocument)
            {
                if (viewElement is View view && !view.IsTemplate && !viewsOnSheets.Contains(view.Id))
                {
                    notInSheets.Add(view);
                }
            }

            foreach (ElementId reference in ExternalFileUtils.GetAllExternalFileReferences(doc))
            {
                Element ext = doc.GetElement(reference);
                externalRefs.Add((ext.GetExternalFileReference().ExternalFileReferenceType, ext.GetExternalFileReference()));
            }

            Dictionary<string, object> results = new()
            {
                { "viewsInsideDocument", viewsInsideDocument.Count },
                { "notInSheets", notInSheets.Count },
                { "annotativeElements", annotativeElements.Count },
                { "externalRefs", externalRefs.Count },
                { "modelGroups", modelGroups.Count },
                { "detailGroups", detailGroups.Count },
                { "designOptions", designOptions.Count },
                { "levels", levels.Count },
                { "grids", grids.Count },
                { "warns", warns.Count },
                { "unenclosedRoom", unenclosedRoom.Count },
                { "viewports", viewports.Count },
                { "unconnectedDucts", unconnectedDucts.Count },
                { "unconnectedPipes", unconnectedPipes.Count },
                { "unconnectedElectrical", unconnectedElectrical.Count },
                { "nonNativeStyles", nonNativeStyles.Count },
                { "isFlipped", isFlipped.Count },
                { "worksetElementCount", worksetElementCount.Count }
            };

            //SendCatchToCallback(new Prey(doc._GetInstancesPerFamily()));
            SendCatchToCallback(new Prey(results));
        }
        private static Dictionary<string, object>? GetCategory(Element e)
        {
            Dictionary<string, object> categoryInfo = [];

            if (e is not null && e.Category is not null)
            {
                categoryInfo.TryAdd("categoryId", e.Category.Id.Value);
                categoryInfo.TryAdd("categoryUniqueId", e.UniqueId ?? Guid.Empty.ToString());
                categoryInfo.TryAdd("categoryName", e.Category.Name ?? string.Empty);
                categoryInfo.TryAdd("builtInCategory", e.Category.BuiltInCategory.ToString() ?? string.Empty);
                categoryInfo.TryAdd("categoryType", e.Category.CategoryType.ToString() ?? string.Empty);
                categoryInfo.TryAdd("hasMaterialQuantities", e.Category.HasMaterialQuantities);
            }
            return categoryInfo;
        }

        private static List<Dictionary<string, object>>? GetParameters(Element e)
        {
            List<Dictionary<string, object>>? results = [];
            if (e is not null && e.GetOrderedParameters() is not null)
            {
                IList<Parameter>? b = e.GetOrderedParameters();
                if (e?.Category is not null)
                {
                    results.AddRange(from p in b
                                     select p.GetParameterValue());
                }
                return results;
            }
            else
            {
                return [];
            }
        }

        private static ElementType? GetElementType(Element e)
        {
            var type = e?.GetTypeId();
            if (type is not null)
            {
                return e?.Document?.GetElement(type) as ElementType;
            }
            return null;
        }

        public override bool Execute()
        {
            ProcessInfo();
            return true;
        }
    }
}
