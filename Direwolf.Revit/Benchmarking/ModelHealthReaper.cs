using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Direwolf.Definitions;
using Direwolf.Revit.Howls;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Benchmarking
{
    public record class ModelHealthReaper : RevitHowl
    {
        private readonly record struct ElementInformation()
        {

            public required double ElementIdValue { get; init; }
            public required string ElementUniqueId { get; init; }
            public required string ElementVersionId { get; init; }
            public string? FamilyName { get; init; }
            public string? Category { get; init; }
            public string? BuiltInCategory { get; init; }
            public string? Workset { get; init; }
            public string[]? Views { get; init; }
            public string? DesignOption { get; init; }
            public string? DocumentOwner { get; init; }
            public string? OwnerViewId { get; init; }
            public string? WorksetId { get; init; }
            public string? LevelId { get; init; }
            public string? CreatedPhaseId { get; init; }
            public string? DemolishedPhaseId { get; init; }
            public string? GroupId { get; init; }
            public string? WorkshareId { get; init; }
            public bool? IsGrouped { get; init; }
            public bool? IsModifiable { get; init; }
            public bool? IsViewSpecific { get; init; }
            public bool? IsBuiltInCategory { get; init; }
            public bool? IsAnnotative { get; init; }
            public bool? IsModel { get; init; }
            public bool? IsPinned { get; init; }
            public bool? IsWorkshared { get; init; }
            public Dictionary<string, string>? Parameters { get; init; }
        }



        private Prey ProcessInfo()
        {
            var doc = GetRevitDocument();

            // Get a very generic collector
            ICollection<Element> collector = [.. new FilteredElementCollector(doc).WhereElementIsNotElementType()];


            // These are all categories for which information has to be extracted.
            List<View> viewsInsideDocument = [];
            List<View> notInSheets = [];
            List<Element> annotativeElements = [];
            List<(ExternalFileReferenceType, ExternalFileReference)> externalRefs = [];
            List<Group> modelGroups = [];
            List<Group> detailGroups = [];
            List<DesignOption> designOptions = [];
            List<Level> levels = [];
            List<Grid> grids = [];
            List<FailureMessage> warns = doc.GetWarnings().ToList();
            List<Room> unenclosedRoom = [];
            List<Viewport> viewports = [];
            List<Duct> unconnectedDucts = [];
            List<Pipe> unconnectedPipes = [];
            List<Connector> unconnectedElectrical = [];
            List<GraphicsStyle> nonNativeStyles = [];
            List<Element> isFlipped = [];
            Dictionary<string, int> worksetElementCount = [];

            var dataCounts = new Dictionary<string, object>
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


            Stack<ElementInformation> individualElementInfo = [];
            Prey genericInformation = new(new Dictionary<string, object>()
            {
                ["counts"] = dataCounts,
                ["elements"] = individualElementInfo
            });


            foreach (Element e in collector)
            {
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
                    if (e.GroupId is not null)
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

                        if (worksetElementCount.ContainsKey(worksetName))
                        {
                            worksetElementCount[worksetName]++;
                        }
                        else
                        {
                            worksetElementCount[worksetName] = 1;
                        }
                    }

                    if (e.LevelId is not null) levelId = e.LevelId.ToString();

                    switch (e)
                    {
                        case (View):
                            View? v = e as View;

                            if (v is not null && !v.IsTemplate)
                            {
                                viewsInsideDocument.Add(v);
                            }
                            builtInCategory = e?.Category?.BuiltInCategory.ToString();
                            break;
                        case (Viewport):
                            Viewport? vp = e as Viewport;
                            if (vp is not null) viewports.Add(vp);
                            builtInCategory = e?.Category?.BuiltInCategory.ToString();
                            break;
                        case (Group):
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
                        case (DesignOption):
                            DesignOption? option = e as DesignOption;
                            if (option is not null) designOptions.Add(option);
                            builtInCategory = e?.Category?.BuiltInCategory.ToString();
                            break;
                        case (Level):
                            Level? l = e as Level;
                            if (l is not null) levels.Add(l);
                            builtInCategory = e?.Category?.BuiltInCategory.ToString();
                            break;
                        case (Grid):
                            Grid? gr = e as Grid;
                            if (gr is not null) grids.Add(gr);
                            builtInCategory = e?.Category?.BuiltInCategory.ToString();
                            break;
                        case (GraphicsStyle):
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
                        case (FamilyInstance):
                            if (fm is not null)
                            {
                                if (fm.Mirrored) isFlipped.Add(e);
                                builtInCategory = e?.Category?.BuiltInCategory.ToString();
                            }
                            break;
                        default:
                            switch (e?.Category?.BuiltInCategory)
                            {
                                case (BuiltInCategory.OST_Rooms):
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
                                case (BuiltInCategory.OST_DuctCurves):
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
                                case (BuiltInCategory.OST_PipeCurves):
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
                                case (BuiltInCategory.OST_ElectricalFixtures):
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
                        case (CategoryType.Model):
                            isAnnotative = false;
                            isModel = true;
                            builtInCategory = CategoryType.Model.ToString();
                            break;
                        case (CategoryType.Annotation):
                            isAnnotative = true;
                            isModel = false;
                            builtInCategory = CategoryType.Annotation.ToString();
                            annotativeElements.Add(e);
                            break;
                        case (CategoryType.Invalid):
                        case (CategoryType.Internal):
                        case (CategoryType.AnalyticalModel):
                        default:
                            break;
                    }

                    individualElementInfo.Push(new ElementInformation
                    {
                        ElementIdValue = e.Id.Value,
                        ElementUniqueId = e.UniqueId,
                        ElementVersionId = e.VersionGuid.ToString(),
                        FamilyName = familyName,
                        Category = builtInCategory,
                        BuiltInCategory = builtInCategory,
                        Workset = workset,
                        Views = views,
                        DesignOption = designOption,
                        DocumentOwner = docOwner,
                        OwnerViewId = ownerViewId,
                        WorksetId = worksetId,
                        LevelId = levelId,
                        CreatedPhaseId = createdPhaseId,
                        DemolishedPhaseId = demolishedPhaseId,
                        GroupId = groupId,
                        WorkshareId = workshareId,
                        IsGrouped = isGrouped,
                        IsModifiable = isModifiable,
                        IsViewSpecific = isViewSpecific,
                        IsBuiltInCategory = isBuiltInCategory,
                        IsAnnotative = isAnnotative,
                        IsModel = isModel,
                        IsPinned = isPinned,
                        IsWorkshared = isWorkshared,
                        Parameters = null
                    });
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
            return genericInformation;
        }


        public override bool Execute()
        {
            SendCatchToCallback(ProcessInfo());
            return true;
        }


    }
}
