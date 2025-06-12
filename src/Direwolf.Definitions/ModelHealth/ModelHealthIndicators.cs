using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Direwolf.Definitions.Drivers;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.Parsers;

namespace Direwolf.Definitions.ModelHealth;

public record ModelHealthIndicators()
{
    public List<Element> AnnotativeElements { get; init; } = [];
    public List<DesignOption> DesignOptions { get; init; } = [];
    public List<Group> DetailGroups { get; init; } = [];
    public List<(ExternalFileReferenceType, ExternalFileReference)> ExternalRefs { get; init; } = [];
    public List<Grid> Grids { get; init; } = [];
    public List<Element> IsFlipped { get; init; } = [];
    public List<Level> Levels { get; init; } = [];
    public List<Group> ModelGroups { get; init; } = [];
    public List<GraphicsStyle> NonNativeStyles { get; init; } = [];
    public List<View> NotInSheets { get; init; } = [];
    public List<Duct> UnconnectedDucts { get; init; } = [];
    public List<Connector> UnconnectedElectrical { get; init; } = [];
    public List<Pipe> UnconnectedPipes { get; init; } = [];
    public List<Room> UnenclosedRoom { get; init; } = [];
    public List<Viewport> Viewports { get; init; } = [];

    public static WolfpackCollectionLegacy Create(Document document, IEnumerable<Element> elementsToCheck)
    {
        var docIdValues = (document.GetDocumentVersionCounter(), document.GetDocumentUuidHash());
        return new WolfpackCollectionLegacy(
            Cuid.CreateRevitId(document, out docIdValues), 
            nameof(ModelHealthIndicators), 
            Method.Get,
            document.GetDocumentUuidHash(), 
            document.GetDocumentVersionHash())
        {
            Payload = new ModelHealthIndicators().GetModelHealthIndicators(document, elementsToCheck)
        };
    }

    private Dictionary<string, object>? GetModelHealthIndicators(Document document,
        IEnumerable<Element> elementsToCheck)
    {
        // These are all categories for which information has to be extracted.
        List<View> _viewsInsideDocument = [];
        List<FailureMessage> _warns = [];
        Dictionary<string, int> _worksetElementCount = [];
        _warns.AddRange(document.GetWarnings());
        foreach (var element in elementsToCheck)
        {
            var e = document.GetElement(element.Id);
            var familyName = string.Empty;
            if ((e is null || !e.IsValidObject || e.Category is null ||
                 e.Category.CategoryType is CategoryType.Invalid) && e?.Category?.CategoryType is CategoryType.Internal)
                continue;
            var worksetParam = e?.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);
            var category = string.Empty;
            var builtInCategory = string.Empty;
            var workset = string.Empty;
            string[]? views = [];
            var designOption = string.Empty;
            var documentOwner = string.Empty;
            var ownerViewId = string.Empty;
            var worksetId = string.Empty;
            var createdPhaseId = string.Empty;
            var demolishedPhaseId = string.Empty;
            var groupId = string.Empty;
            var workshareId = string.Empty;
            var levelId = string.Empty;
            bool? isGrouped = false;
            bool? isModifiable = false;
            bool? isViewSpecific = false;
            bool? isBuiltInCategory = false;
            bool? isAnnotative = false;
            bool? isModel = false;
            bool? isPinned = false;
            bool? isWorkshared = false;
            var fm = e as FamilyInstance;

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
            if (e.Document is not null) documentOwner = document.CreationGUID.ToString();
            isPinned = e.Pinned;
            if (document.IsWorkshared)
            {
                isWorkshared = true;
                workshareId = document.WorksharingCentralGUID.ToString();
            }

            if (worksetParam != null)
            {
                var worksetName = worksetParam.AsValueString();
                if (_worksetElementCount.TryGetValue(worksetName, out var value))
                    _worksetElementCount[worksetName] = ++value;
                else
                    _worksetElementCount[worksetName] = 1;
            }

            if (e.LevelId is not null) levelId = e.LevelId.ToString();
            switch (e)
            {
                case View:
                    if (e is View v && !v.IsTemplate) _viewsInsideDocument.Add(v);
                    builtInCategory = e?.Category?.BuiltInCategory.ToString();
                    break;
                case Viewport:
                    if (e is Viewport vp) Viewports.Add(vp);
                    builtInCategory = e?.Category?.BuiltInCategory.ToString();
                    break;
                case Group:
                    if (e is Group g)
                    {
                        if (g.Category.Name == "Detail Groups")
                            DetailGroups.Add(g);
                        else
                            ModelGroups.Add(g);
                    }

                    builtInCategory = e?.Category?.BuiltInCategory.ToString();
                    break;
                case DesignOption:
                    if (e is DesignOption option) DesignOptions.Add(option);
                    builtInCategory = e?.Category?.BuiltInCategory.ToString();
                    break;
                case Level:
                    if (e is Level l) Levels.Add(l);
                    builtInCategory = e?.Category?.BuiltInCategory.ToString();
                    break;
                case Grid:
                    if (e is Grid gr) Grids.Add(gr);
                    builtInCategory = e?.Category?.BuiltInCategory.ToString();
                    break;
                case GraphicsStyle:
                    if (e is GraphicsStyle graphicsStyle)
                    {
                        var c = graphicsStyle.GraphicsStyleCategory;
                        builtInCategory = e?.Category?.BuiltInCategory.ToString();
                        if (c is not null && c.IsCuttable is not false && c.CategoryType == CategoryType.Annotation)
                            NonNativeStyles.Add(graphicsStyle);
                    }

                    break;
                case FamilyInstance:
                    if (fm is not null)
                    {
                        if (fm.Mirrored) IsFlipped.Add(e);
                        builtInCategory = e?.Category?.BuiltInCategory.ToString();
                    }

                    break;
                default:
                    switch (e?.Category?.BuiltInCategory)
                    {
                        case BuiltInCategory.OST_Rooms:
                            if (e is Room room)
                            {
                                var boundarySegments =
                                    room.GetBoundarySegments(new SpatialElementBoundaryOptions());
                                if (boundarySegments == null || boundarySegments.Count == 0) UnenclosedRoom.Add(room);
                            }

                            builtInCategory = e?.Category?.BuiltInCategory.ToString();
                            break;
                        case BuiltInCategory.OST_DuctCurves:
                            if (e is Duct duct)
                            {
                                var connectors = duct.ConnectorManager.Connectors;
                                var isUnconnected = connectors.Cast<Connector>()
                                    .Any(connector => !connector.IsConnected);
                                if (isUnconnected) UnconnectedDucts.Add(duct);
                            }

                            builtInCategory = e?.Category?.BuiltInCategory.ToString();
                            break;
                        case BuiltInCategory.OST_PipeCurves:
                            if (e is Pipe pipe)
                            {
                                var connectors = pipe.ConnectorManager.Connectors;
                                var isUnconnected = connectors.Cast<Connector>()
                                    .Any(connector => !connector.IsConnected);
                                if (isUnconnected) UnconnectedPipes.Add(pipe);
                            }

                            builtInCategory = e?.Category?.BuiltInCategory.ToString();
                            break;
                        case BuiltInCategory.OST_ElectricalFixtures:
                            var mepModel = ((FamilyInstance)e).MEPModel;
                            if (mepModel != null)
                            {
                                var connectors = mepModel.ConnectorManager.Connectors;
                                foreach (Connector connector in connectors)
                                {
                                    if (!connector.IsConnected) UnconnectedElectrical.Add(connector);
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
                    AnnotativeElements.Add(e);
                    break;
                case CategoryType.Invalid:
                case CategoryType.Internal:
                case CategoryType.AnalyticalModel:
                default:
                    break;
            }

            // view not in sheet. needs to be done after all are done.
            foreach (var viewElement in _viewsInsideDocument)
            {
                if (viewElement is { IsTemplate: false } && !_viewsInsideDocument.Contains(viewElement))
                    NotInSheets.Add(viewElement);
            }
            foreach (var reference in ExternalFileUtils.GetAllExternalFileReferences(document))
            {
                using var ext = document.GetElement(reference);
                ExternalRefs.Add((ext.GetExternalFileReference().ExternalFileReferenceType,
                    ext.GetExternalFileReference()));
            }

            // HashSet<ElementId> viewsOnSheets = [.. Viewports.Select(vp => vp.ViewId)];
        }

        return new Dictionary<string, object>()
        {
            { "viewsInsideDocument", _viewsInsideDocument.Count },
            { "notInSheets", NotInSheets.Count },
            { "annotativeElements", AnnotativeElements.Count },
            { "externalRefs", ExternalRefs.Count },
            { "modelGroups", ModelGroups.Count },
            { "detailGroups", DetailGroups.Count },
            { "designOptions", DesignOptions.Count },
            { "levels", Levels.Count },
            { "grids", Grids.Count },
            { "warns", _warns.Count },
            { "unenclosedRoom", UnenclosedRoom.Count },
            { "viewports", Viewports.Count },
            { "unconnectedDucts", UnconnectedDucts.Count },
            { "unconnectedPipes", UnconnectedPipes.Count },
            { "unconnectedElectrical", UnconnectedElectrical.Count },
            { "nonNativeStyles", NonNativeStyles.Count },
            { "isFlipped", IsFlipped.Count },
            { "worksetElementCount", _worksetElementCount.Count },
        };
    }
}