//using Autodesk.Revit.DB;
//using Autodesk.Revit.DB.Architecture;
//using Autodesk.Revit.DB.Mechanical;
//using Autodesk.Revit.DB.Plumbing;
//using Direwolf.Definitions;
//using Direwolf.Revit.Definitions;
//using Direwolf.Revit.Utilities;
//using Microsoft.EntityFrameworkCore.Metadata.Conventions;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection.Metadata;
//using System.Text;
//using System.Threading.Tasks;
//using Document = Autodesk.Revit.DB.Document;

//namespace Direwolf.Revit.Howls
//{
//    public record class GetModelSnapshotWithInfo : RevitHowl
//    {
//        public GetModelSnapshotWithInfo(Document doc) => SetRevitDocument(doc);
//        // These are all categories for which information has to be extracted.
//        private List<ElementInformation> viewsInsideDocument = [];
//        private List<ElementInformation> notInSheets = [];
//        private List<ElementInformation> annotativeElements = [];
//        private List<(ExternalFileReferenceType, ExternalFileReference)> externalRefs = [];
//        private List<ElementInformation> modelGroups = [];
//        private List<ElementInformation> detailGroups = [];
//        private List<ElementInformation> designOptions = [];
//        private List<ElementInformation> levels = [];
//        private List<ElementInformation> grids = [];
//        private List<ElementInformation> warns = [];
//        private List<ElementInformation> unenclosedRoom = [];
//        private List<ElementInformation> viewports = [];
//        private List<ElementInformation> unconnectedDucts = [];
//        private List<ElementInformation> unconnectedPipes = [];
//        private List<ElementInformation> unconnectedElectrical = [];
//        private List<ElementInformation> nonNativeStyles = [];
//        private List<ElementInformation> isFlipped = [];
//        private List<ElementInformation> worksetElementCount = [];
//        private Stack<ElementInformation> individualElementInfo = [];

//        private Prey ProcessInfo()
//        {
//            var doc = GetRevitDocument();
//            ICollection<Element> collector = [.. new FilteredElementCollector(GetRevitDocument()).WhereElementIsNotElementType()];

//            foreach (Element e in collector)
//            {
//                if (e is not null && e.IsValidObject && e.category is not null && e.category.CategoryType is not CategoryType.Invalid || e?.category?.CategoryType is not CategoryType.Internal)
//                {
//                    Autodesk.Revit.DB.Parameter? worksetParam = e?.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);

//                    string? familyName = string.Empty;
//                    string? category = string.Empty;
//                    string? builtInCategory = string.Empty;
//                    string? workset = string.Empty;
//                    string[]? views = [];
//                    string? designOption = string.Empty;
//                    string? docOwner = string.Empty;
//                    string? ownerViewId = string.Empty;
//                    string? worksetId = string.Empty;
//                    string? createdPhaseId = string.Empty;
//                    string? demolishedPhaseId = string.Empty;
//                    string? groupId = string.Empty;
//                    string? workshareId = string.Empty;
//                    string levelId = string.Empty;
//                    bool? isGrouped = false;
//                    bool? isModifiable = false;
//                    bool? isViewSpecific = false;
//                    bool? isBuiltInCategory = false;
//                    bool? isAnnotative = false;
//                    bool? isModel = false;
//                    bool? isPinned = false;
//                    bool? isWorkshared = false;

//                    FamilyInstance? fm = e as FamilyInstance;

//                    // isGrouped
//                    if (e?.groupId is not null)
//                    {
//                        isGrouped = true;
//                        groupId = e.groupId.ToString();
//                    }

//                    // isModifiable
//                    if (e.isModifiable) isModifiable = true;

//                    // isViewSpecific
//                    if (!e.ViewSpecific)
//                    {
//                        familyName = fm?.Symbol.Family.Name;
//                    }
//                    else
//                    {
//                        isViewSpecific = true;
//                        ownerViewId = e.ownerViewId.ToString();
//                    }

//                    // isBuiltInCategory + builtInCategory
//                    if (e.category is not null && e.category.builtInCategory is not builtInCategory.INVALID)
//                    {
//                        isBuiltInCategory = true;
//                        category = e.category.Name;
//                    }

//                    if (e.worksetId is not null) worksetId = e.worksetId.ToString();

//                    if (e.HasPhases())
//                    {
//                        if (e.createdPhaseId is not null) createdPhaseId = e.createdPhaseId.ToString();
//                        if (e.demolishedPhaseId is not null) demolishedPhaseId = e.demolishedPhaseId.ToString();
//                    }

//                    if (e.designOption is not null) designOption = e.designOption.Name;

//                    if (e.Document is not null) docOwner = doc.CreationGUID.ToString();

//                    isPinned = e.Pinned;

//                    if (doc.isWorkshared)
//                    {
//                        isWorkshared = true;
//                        workshareId = doc.WorksharingCentralGUID.ToString();
//                    }

//                    if (worksetParam != null)
//                    {
//                        string worksetName = worksetParam.AsValueString();

//                        if (worksetElementCount.TryGetValue(worksetName, out int value))
//                        {
//                            worksetElementCount[worksetName] = ++value;
//                        }
//                        else
//                        {
//                            worksetElementCount[worksetName] = 1;
//                        }
//                    }

//                    if (e.levelId is not null) levelId = e.levelId.ToString();

//                    switch (e)
//                    {
//                        case View:
//                            View? v = e as View;

//                            if (v is not null && !v.IsTemplate)
//                            {
//                                viewsInsideDocument.Add(v);
//                            }
//                            builtInCategory = e?.category?.builtInCategory.ToString();
//                            break;
//                        case Viewport:
//                            Viewport? vp = e as Viewport;
//                            if (vp is not null) viewports.Add(vp);
//                            builtInCategory = e?.category?.builtInCategory.ToString();
//                            break;
//                        case Group:
//                            Group? g = e as Group;
//                            if (g is not null)
//                            {
//                                if (g.category.Name == "Detail Groups")
//                                {
//                                    detailGroups.Add(g);
//                                }
//                                else
//                                {
//                                    modelGroups.Add(g);
//                                }
//                            }
//                            builtInCategory = e?.category?.builtInCategory.ToString();
//                            break;
//                        case designOption:
//                            designOption? option = e as designOption;
//                            if (option is not null) designOptions.Add(option);
//                            builtInCategory = e?.category?.builtInCategory.ToString();
//                            break;
//                        case Level:
//                            Level? l = e as Level;
//                            if (l is not null) levels.Add(l);
//                            builtInCategory = e?.category?.builtInCategory.ToString();
//                            break;
//                        case Grid:
//                            Grid? gr = e as Grid;
//                            if (gr is not null) grids.Add(gr);
//                            builtInCategory = e?.category?.builtInCategory.ToString();
//                            break;
//                        case GraphicsStyle:
//                            GraphicsStyle? graphicsStyle = e as GraphicsStyle;
//                            if (graphicsStyle is not null)
//                            {
//                                category c = graphicsStyle.GraphicsStyleCategory;
//                                builtInCategory = e?.category?.builtInCategory.ToString();

//                                if (c is not null && c.IsCuttable is not false && c.CategoryType == CategoryType.Annotation)
//                                {
//                                    nonNativeStyles.Add(graphicsStyle);
//                                }
//                            }
//                            break;
//                        case FamilyInstance:
//                            if (fm is not null)
//                            {
//                                if (fm.Mirrored) isFlipped.Add(e);
//                                builtInCategory = e?.category?.builtInCategory.ToString();
//                            }
//                            break;
//                        default:
//                            switch (e?.category?.builtInCategory)
//                            {
//                                case builtInCategory.OST_Rooms:
//                                    Room? room = e as Room;
//                                    if (room is not null)
//                                    {
//                                        IList<IList<BoundarySegment>> boundarySegments = room.GetBoundarySegments(new SpatialElementBoundaryOptions());

//                                        if (boundarySegments == null || boundarySegments.Count == 0)
//                                        {
//                                            unenclosedRoom.Add(room);
//                                        }
//                                    }
//                                    builtInCategory = e?.category?.builtInCategory.ToString();

//                                    break;
//                                case builtInCategory.OST_DuctCurves:
//                                    if (e is Duct duct)
//                                    {
//                                        bool isUnconnected = false;

//                                        ConnectorSet connectors = duct.ConnectorManager.Connectors;
//                                        foreach (Connector connector in connectors)
//                                        {
//                                            if (!connector.IsConnected)
//                                            {
//                                                isUnconnected = true;
//                                                break;
//                                            }
//                                        }

//                                        if (isUnconnected)
//                                        {
//                                            unconnectedDucts.Add(duct);
//                                        }
//                                    }
//                                    builtInCategory = e?.category?.builtInCategory.ToString();
//                                    break;
//                                case builtInCategory.OST_PipeCurves:
//                                    if (e is Pipe pipe)
//                                    {
//                                        bool isUnconnected = false;

//                                        ConnectorSet connectors = pipe.ConnectorManager.Connectors;
//                                        foreach (Connector connector in connectors)
//                                        {
//                                            if (!connector.IsConnected)
//                                            {
//                                                isUnconnected = true;
//                                                break;
//                                            }
//                                        }

//                                        if (isUnconnected)
//                                        {
//                                            unconnectedPipes.Add(pipe);
//                                        }
//                                    }
//                                    builtInCategory = e?.category?.builtInCategory.ToString();
//                                    break;
//                                case builtInCategory.OST_ElectricalFixtures:
//                                    MEPModel mepModel = ((FamilyInstance)e).MEPModel;
//                                    if (mepModel != null)
//                                    {
//                                        ConnectorSet connectors = mepModel.ConnectorManager.Connectors;
//                                        foreach (Connector connector in connectors)
//                                        {
//                                            if (!connector.IsConnected)
//                                            {
//                                                unconnectedElectrical.Add(connector);
//                                            }
//                                        }
//                                    }
//                                    builtInCategory = e?.category?.builtInCategory.ToString();
//                                    break;
//                                default:
//                                    builtInCategory = e?.category?.builtInCategory.ToString();
//                                    break;
//                            }

//                            //TODO: check for unused
//                            //Debug.Print(e.Name + "" + typeof(Element).Name);
//                            break;
//                    }

//                    switch (e?.category?.CategoryType)
//                    {
//                        case CategoryType.Model:
//                            isAnnotative = false;
//                            isModel = true;
//                            builtInCategory = CategoryType.Model.ToString();
//                            break;
//                        case CategoryType.Annotation:
//                            isAnnotative = true;
//                            isModel = false;
//                            builtInCategory = CategoryType.Annotation.ToString();
//                            annotativeElements.Add(e);
//                            break;
//                        case CategoryType.Invalid:
//                        case CategoryType.Internal:
//                        case CategoryType.AnalyticalModel:
//                        default:
//                            break;
//                    }

//                    individualElementInfo.Push(new ElementInformation
//                    {
//                        idValue = e.Id.Value,
//                        uniqueElementId = e.UniqueId,
//                        elementVersionId = e.VersionGuid.ToString(),
//                        familyName = familyName,
//                        category = builtInCategory,
//                        builtInCategory = builtInCategory,
//                        workset = workset,
//                        views = views,
//                        designOption = designOption,
//                        documentOwner = docOwner,
//                        ownerViewId = ownerViewId,
//                        worksetId = worksetId,
//                        levelId = levelId,
//                        createdPhaseId = createdPhaseId,
//                        demolishedPhaseId = demolishedPhaseId,
//                        groupId = groupId,
//                        workshareId = workshareId,
//                        isGrouped = isGrouped,
//                        isModifiable = isModifiable,
//                        isViewSpecific = isViewSpecific,
//                        isBuiltInCategory = isBuiltInCategory,
//                        isAnnotative = isAnnotative,
//                        isModel = isModel,
//                        isPinned = isPinned,
//                        isWorkshared = isWorkshared,
//                        Parameters = null
//                    });
//                }
//            }
//            // view not in sheet. needs to be done after all are done.
//            HashSet<ElementId> viewsOnSheets = [.. viewports.Select(vp => (vp as Viewport).ViewId)];
//            foreach (Element viewElement in viewsInsideDocument)
//            {
//                if (viewElement is View view && !view.IsTemplate && !viewsOnSheets.Contains(view.Id))
//                {
//                    notInSheets.Add(view);
//                }
//            }

//            foreach (ElementId reference in ExternalFileUtils.GetAllExternalFileReferences(doc))
//            {
//                Element ext = doc.GetElement(reference);
//                externalRefs.Add((ext.GetExternalFileReference().ExternalFileReferenceType, ext.GetExternalFileReference()));
//            }

//            List<ElementInformation> views = [];
//            views.AddRange(from v in viewsInsideDocument
//                           let e = v as Element
//                           select DirewolfExtensions.GetElementInformation(e, doc));

//            List<ElementInformation> sheets = [];



//            Dictionary<string, object> results = new()
//            {
//                { "viewsInsideDocument", viewsInsideDocument.SelectMany(x => DirewolfExtensions.GetElementInformation(x, GetRevitDocument()))},
//                { "notInSheets", notInSheets.Count },
//                { "annotativeElements", annotativeElements.Count },
//                { "externalRefs", externalRefs.Count },
//                { "modelGroups", modelGroups.Count },
//                { "detailGroups", detailGroups.Count },
//                { "designOptions", designOptions.Count },
//                { "levels", levels.Count },
//                { "grids", grids.Count },
//                { "warns", warns.Count },
//                { "unenclosedRoom", unenclosedRoom.Count },
//                { "viewports", viewports.Count },
//                { "unconnectedDucts", unconnectedDucts.Count },
//                { "unconnectedPipes", unconnectedPipes.Count },
//                { "unconnectedElectrical", unconnectedElectrical.Count },
//                { "nonNativeStyles", nonNativeStyles.Count },
//                { "isFlipped", isFlipped.Count },
//                { "worksetElementCount", worksetElementCount.Count }
//            };
//            return new Prey(results);
//        }

//        public override bool Execute()
//        {

//        }
//    }
//}
