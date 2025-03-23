using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Direwolf.Definitions;
using Direwolf.Revit.Definitions;
using System;
using Direwolf.Revit.Definitions.Legacy;
using Direwolf.Revit.Extensions;

namespace Direwolf.Revit.Howls.ModelHealth
{
    public record class GetModelSnapshot : RevitHowl
    {

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
        private List<ElementIntrospection> individualElementInfo = [];

        private Prey ProcessInfo()
        {
            var doc = GetRevitDocument();
            warns.AddRange(GetRevitDocument().GetWarnings());
            ICollection<Element> collector = [.. new FilteredElementCollector(GetRevitDocument()).WhereElementIsNotElementType()];

            foreach (Element e in collector)
            {
                if (e is not null && e.IsValidObject && e?.Category is not null && e?.Category.CategoryType is not CategoryType.Invalid || e?.Category?.CategoryType is not CategoryType.Internal)
                {
                    // the checker goes one by one checking if any of these conditions is true
                    // or have a value assigned.
                    // -1 is the default ID number used internally by Revit to denote invalid elements
                    // so, in this case, I use the same nomenclature.
                    // that way it's easier to check something's invalid.

                    // this one is *crucial* to avoid an exception that stops the whole Reaper.
                    // Revit throws an exception if we ever *dare* to get a workshare value if it's not
                    // a workshared doc.
                    Parameter? worksetParam = e?.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);
                    string? familyName = string.Empty;
                    string? category = string.Empty;
                    string? builtInCategory = string.Empty;
                    string[]? views = [];
                    string? designOption = string.Empty;
                    string? docOwner = string.Empty;
                    double? ownerViewId = -1;
                    int? worksetId = -1;
                    double? createdPhaseId = -1;
                    double? demolishedPhaseId = -1;
                    double? groupId = -1;
                    string? workshareId = string.Empty;
                    string? location = string.Empty;
                    double? levelId = -1;
                    bool? isGrouped = false;
                    bool? isModifiable = false;
                    bool? isViewSpecific = false;
                    bool? isBuiltInCategory = false;
                    bool? isAnnotative = false;
                    bool? isModel = false;
                    bool? isPinned = false;
                    bool? isWorkshared = false;
                    bool? hasDesignOption = false;

                    FamilyInstance? fm = e as FamilyInstance;

                    // these could crash the checker if not set correctly *per element*
                    // if an element passes this without throwing an exception
                    // it's gone through a thorough sieve of possible candidates.
                    #region spice level checks
                    // haslocation
                    if (e?.Location is not null) location = e?.Location.ToString();

                    if (e?.DesignOption is not null) hasDesignOption = true;

                    // isGrouped
                    if (e?.GroupId is not null)
                    {
                        isGrouped = true;
                        groupId = e?.GroupId.Value;
                    }

                    // isModifiable
                    if (e is not null && e.IsModifiable) isModifiable = true;

                    // isViewSpecific
                    if (e is not null && !e.ViewSpecific)
                    {
                        familyName = fm?.Symbol.Family.Name;
                    }
                    else
                    {
                        isViewSpecific = true;
                        ownerViewId = e?.OwnerViewId.Value;
                    }

                    // isBuiltInCategory + builtInCategory
                    if (e?.Category is not null && e.Category.BuiltInCategory is not BuiltInCategory.INVALID)
                    {
                        isBuiltInCategory = true;
                        category = e?.Category.Name;
                    }

                    // phases
                    if (e is not null && e.HasPhases())
                    {
                        if (e is not null && e?.CreatedPhaseId is not null) createdPhaseId = e?.CreatedPhaseId.Value;
                        if (e is not null && e?.DemolishedPhaseId is not null) demolishedPhaseId = e?.DemolishedPhaseId.Value;
                    }

                    // designOptions
                    if (e is not null && e?.DesignOption is not null) designOption = e?.DesignOption.Name;

                    // creationGuid
                    if (e is not null && e?.Document is not null) docOwner = doc.CreationGUID.ToString();

                    // pinned
                    isPinned = e?.Pinned;

                    // workshared
                    if (doc.IsWorkshared)
                    {
                        isWorkshared = true;
                        workshareId = doc.WorksharingCentralGUID.ToString();
                        // workset
                        if (e?.WorksetId is not null) worksetId = e?.WorksetId.IntegerValue;
                        // elements by workset
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
                    }

                    // level
                    if (e is not null && e?.LevelId is not null) levelId = e.LevelId.Value;


                    #region filter
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
                            break;
                            #endregion
                    }
                    #region parameters
                    List<ParameterIntrospection> parameters = [];

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

                    string getParameterData(Parameter parameter)
                    {
                        return parameter.StorageType switch
                        {
                            StorageType.String => parameter.AsString(),
                            StorageType.Integer => parameter.AsInteger().ToString(),
                            StorageType.Double => parameter.AsDouble().ToString(),
                            StorageType.ElementId => parameter.AsElementId().ToString(),
                            _ => string.Empty,
                        };
                    }

                    if (e?.GetOrderedParameters() is not null)
                    {
                        foreach (Parameter? p in e?.GetOrderedParameters())
                        {
                            try
                            {
                                parameters.Add(new ParameterIntrospection()
                                {
                                    id = TryGetSafe(() => p.Id.Value, -1),
                                    name = TryGetSafe(() => p.Definition.Name, string.Empty),
                                    value = TryGetSafe(() => getParameterData(p), string.Empty),
                                    storageType = TryGetSafe(() => p.StorageType.ToString(), string.Empty),
                                    unitTypeId = TryGetSafe(() => p.GetUnitTypeId().TypeId, string.Empty),
                                    parentElement = TryGetSafe(() => p.Element.UniqueId.ToString(), Guid.Empty.ToString()),
                                    hasValue = TryGetSafe(() => p.HasValue, false),
                                    userModifiable = TryGetSafe(() => p.UserModifiable, false),
                                    isShared = TryGetSafe(() => p.IsShared, false),
                                    sharedParameterGuid = TryGetSafe(() => p.GUID.ToString(), Guid.Empty.ToString()),
                                    dataType = TryGetSafe(() => p.Definition.GetDataType().TypeId, string.Empty),
                                    groupTypeId = TryGetSafe(() => p.Definition.GetGroupTypeId().TypeId, string.Empty)
                                });
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                    //individualElementInfo.Push(new _ElementInformation
                    //{
                    //    idValue = e.Id.Value,
                    //    uniqueElementId = e.UniqueId,
                    //    elementVersionId = e.VersionGuid.ToString(),
                    //    familyName = familyName,
                    //    category = builtInCategory,
                    //    builtInCategory = builtInCategory,
                    //    workset = workset,
                    //    views = views,
                    //    designOption = designOption,
                    //    documentOwner = docOwner,
                    //    ownerViewId = ownerViewId,
                    //    worksetId = worksetId,
                    //    levelId = levelId,
                    //    createdPhaseId = createdPhaseId,
                    //    demolishedPhaseId = demolishedPhaseId,
                    //    groupId = groupId,
                    //    workshareId = workshareId,
                    //    isGrouped = isGrouped,
                    //    isModifiable = isModifiable,
                    //    isViewSpecific = isViewSpecific,
                    //    isBuiltInCategory = isBuiltInCategory,
                    //    isAnnotative = isAnnotative,
                    //    isModel = isModel,
                    //    isPinned = isPinned,
                    //    isWorkshared = isWorkshared,
                    //    Parameters = null
                    //});

                    #endregion

                    #region element
                    individualElementInfo.Add(new ElementIntrospection()
                    {
                        id = e?.Id.Value,

                        isGrouped = isGrouped,
                        isModifiable = isModifiable,
                        isViewSpecific = isViewSpecific,
                        isBuiltInCategory = isBuiltInCategory,
                        isAnnotative = isAnnotative,
                        isModel = isModel,
                        isPinned = isPinned,
                        isWorkshared = isWorkshared,
                        hasDesignOption = hasDesignOption,

                        assemblyInstanceId = TryGetSafe(() => e.AssemblyInstanceId.Value, -1),
                        createdPhaseId = createdPhaseId,
                        demolishedPhaseId = demolishedPhaseId,
                        groupId = groupId,
                        levelId = levelId,
                        ownerViewId = ownerViewId,

                        worksetId = worksetId,

                        uniqueId = e?.UniqueId,
                        elementVersionId = e?.VersionGuid.ToString(),
                        builtInCategory = builtInCategory,
                        location = location,
                        documentOwnerId = docOwner,
                        name = TryGetSafe(() => e.Name, string.Empty),
                        workshareId = workshareId,
                        parameters = parameters

                        //id = e.Id.Value,
                        //uniqueId = e.UniqueId,
                        //elementVersionId = e.VersionGuid.ToString(),
                        //builtInCategory = builtInCategory,
                        //worksetId = worksetId,
                        //documentOwnerId = docOwner,
                        //ownerViewId = ownerViewId,
                        //levelId = levelId,
                        //createdPhaseId = createdPhaseId,
                        //demolishedPhaseId = demolishedPhaseId,
                        //groupId = groupId,
                        //workshareId = workshareId,
                        //isGrouped = isGrouped,
                        //isModifiable = isModifiable,
                        //isViewSpecific = isViewSpecific,
                        //isBuiltInCategory = isBuiltInCategory,
                        //isAnnotative = isAnnotative,
                        //isModel = isModel,
                        //isPinned = isPinned,
                        //isWorkshared = isWorkshared,
                        //parameters = parameters,
                        //assemblyInstanceId = TryGetSafe(() => e.AssemblyInstanceId.Value, -1)
                    });
                    #endregion
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

            ProjectInformationIntrospection pji = new()
            {
                projectName = TryGetSafe(() => doc.ProjectInformation.Name, string.Empty),
                client = TryGetSafe(() => doc.ProjectInformation.ClientName,
                string.Empty),
                address = TryGetSafe(() => doc.ProjectInformation.Address, string.Empty),
                author = TryGetSafe(() => doc.ProjectInformation.Author, string.Empty),
                buildingName = TryGetSafe(() => doc.ProjectInformation.BuildingName, string.Empty),
                issueDate = TryGetSafe(() => doc.ProjectInformation.IssueDate, string.Empty),
                location = TryGetSafe(() => doc.ProjectInformation.Location.ToString() ?? string.Empty, string.Empty),
                projectNumber = TryGetSafe(() => doc.ProjectInformation.Number, string.Empty),
                organizationDescription = TryGetSafe(() => doc.ProjectInformation.OrganizationDescription, string.Empty),
                organizationName = TryGetSafe(() => doc.ProjectInformation.OrganizationName, string.Empty),
                status = TryGetSafe(() => doc.ProjectInformation.Status, string.Empty)
            };

            DocumentIntrospection d = new()
            {
                documentName = TryGetSafe(() => doc.Title, string.Empty),
                documentPath = TryGetSafe(() => doc.PathName, string.Empty),
                documentUniqueId = TryGetSafe(() => doc.CreationGUID.ToString(), string.Empty),
                documentVersionId = TryGetSafe(() => doc.ProjectInformation.VersionGuid.ToString(), string.Empty),
                documentSaveCount = TryGetSafe(() => Document.GetDocumentVersion(doc).NumberOfSaves, 0),
                warnings = [.. warns.Select(x => x.GetDescriptionText())],
                projectInformation = pji
            };

            return new Prey(results);
        }

        private static string TryGetSafe(Func<string> func, string defaultValue)
        {
            try { return func(); } catch { return defaultValue; }
        }
        private static int TryGetSafe(Func<int> func, int defaultValue)
        {
            try { return func(); } catch { return defaultValue; }
        }
        private static double TryGetSafe(Func<double> func, double defaultValue)
        {
            try { return func(); } catch { return defaultValue; }
        }
        private static bool TryGetSafe(Func<bool> func, bool defaultValue)
        {
            try { return func(); } catch { return defaultValue; }
        }


        public override bool Execute()
        {
            SendCatchToCallback(ProcessInfo());
            return true;
        }
    }
}
//Dictionary<string, object> results = new()
//{
//    { "viewsInsideDocument", viewsInsideDocument.Count },
//    { "notInSheets", notInSheets.Count },
//    { "annotativeElements", annotativeElements.Count },
//    { "externalRefs", externalRefs.Count },
//    { "modelGroups", modelGroups.Count },
//    { "detailGroups", detailGroups.Count },
//    { "designOptions", designOptions.Count },
//    { "levels", levels.Count },
//    { "grids", grids.Count },
//    { "warns", warns.Count },
//    { "unenclosedRoom", unenclosedRoom.Count },
//    { "viewports", viewports.Count },
//    { "unconnectedDucts", unconnectedDucts.Count },
//    { "unconnectedPipes", unconnectedPipes.Count },
//    { "unconnectedElectrical", unconnectedElectrical.Count },
//    { "nonNativeStyles", nonNativeStyles.Count },
//    { "isFlipped", isFlipped.Count },
//    { "worksetElementCount", worksetElementCount.Count }
//};

