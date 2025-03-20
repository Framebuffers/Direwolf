//using Autodesk.Revit.DB;
//using Autodesk.Revit.DB.Architecture;
//using Autodesk.Revit.DB.Mechanical;
//using Autodesk.Revit.DB.Plumbing;
//using Direwolf.Definitions;
//using Direwolf.Revit.Definitions;
//using Direwolf.Revit.Extensions;
//using System.Diagnostics;
//using System.Text.Json;

//namespace Direwolf.Revit.Howls
//{
//    /// <summary>
//    /// This is an unbundled version of all tests inside Direwolf Revit.
//    /// It runs a very exhaustive analysis of the whole model; iterating, categorizing, extracting data and saving records of each element inside the model.
//    /// This is the most intense test in all of Direwolf Revit.
//    /// It outputs a record called <see cref="ModelIntrospection"/>, which contains not only the counts, but also a <see cref="ElementInformation"/> and <see cref="ParameterInformation"/> for each element.
//    /// Output of this file is meant to go into three tables inside the Direwolf database:
//    ///     - <see cref="Wolfpack"/>
//    ///         |_<see cref="ModelIntrospection"/> <-- Goes into [results] as JSON
//    ///         |_<see cref="ElementInformation"/> <-- Dedicated table with FK to Wolfpack's row
//    ///             |_<see cref="ParameterInformation"/>  <-- Dedicated table with FK to their corresponding <see cref="ElementInformation"/>.
//    /// Check <see cref="WolfpackDB"/> for details on the SQL implementation. Schema is inside the prisma.schema file on direwolf-db.
//    /// </summary>
//    public record class _getModelSnapshot : RevitHowl
//    {
//        public _getModelSnapshot(Document doc) => SetRevitDocument(doc);
//        int failCounter = 0;
//        // These are all categories for which information has to be extracted.
//        private List<View> viewsInsideDocument = [];
//        private List<View> notInSheets = [];
//        private List<Element> annotativeElements = [];
//        private List<(ExternalFileReferenceType, ExternalFileReference)> externalRefs = [];
//        private List<Group> modelGroups = [];
//        private List<Group> detailGroups = [];
//        private List<DesignOption> designOptions = [];
//        private List<Level> levels = [];
//        private List<Grid> grids = [];
//        private List<FailureMessage> warns = [];
//        private List<Room> unenclosedRoom = [];
//        private List<Viewport> viewports = [];
//        private List<Duct> unconnectedDucts = [];
//        private List<Pipe> unconnectedPipes = [];
//        private List<Connector> unconnectedElectrical = [];
//        private List<GraphicsStyle> nonNativeStyles = [];
//        private List<Element> isFlipped = [];
//        private Dictionary<string, int> worksetElementCount = [];
//        private Stack<ElementInformation> individualElementInfo = [];
//        private Dictionary<Family, List<Element?>> getElementsByFamily = [];
//        private Dictionary<string, List<string>> elementsByFamilyString = [];

//        private Prey ProcessInfo()
//        {
//            var doc = GetRevitDocument();
//            warns.AddRange(GetRevitDocument().GetWarnings());
//            ICollection<Element> collector = [.. new FilteredElementCollector(GetRevitDocument()).WhereElementIsNotElementType()];
//            double elementCountTotal = collector.Count;

//            foreach (Element e in collector)
//            {
//                try
//                {
//                    if (e is not null && e.IsValidObject && e.Category is not null && e.Category.CategoryType is not CategoryType.Invalid || e?.Category?.CategoryType is not CategoryType.Internal)
//                    {
//                        Parameter? worksetParam = e?.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);

//                        string? familyName = string.Empty;
//                        string? category = string.Empty;
//                        string? builtInCategory = string.Empty;
//                        string? workset = string.Empty;
//                        string[]? views = [];
//                        string? designOption = string.Empty;
//                        string? docOwner = string.Empty;
//                        string? ownerViewId = string.Empty;
//                        string? worksetId = string.Empty;
//                        string? createdPhaseId = string.Empty;
//                        string? demolishedPhaseId = string.Empty;
//                        string? groupId = string.Empty;
//                        string? workshareId = string.Empty;
//                        string levelId = string.Empty;
//                        bool? isGrouped = false;
//                        bool? isModifiable = false;
//                        bool? isViewSpecific = false;
//                        bool? isBuiltInCategory = false;
//                        bool? isAnnotative = false;
//                        bool? isModel = false;
//                        bool? isPinned = false;
//                        bool? isWorkshared = false;

//                        FamilyInstance? fm = e as FamilyInstance;
//                        if (fm is not null)
//                        {
//                            if (!getElementsByFamily.TryGetValue(fm.Symbol.Family, out List<Element?>? value))
//                            {
//                                value = [];
//                                getElementsByFamily[fm.Symbol.Family] = value;
//                            }
//                            value.Add(fm.Symbol.Family);

//                            if (!elementsByFamilyString.TryGetValue(fm.Symbol.FamilyName, out List<string>? stringValue))
//                            {
//                                stringValue = [];
//                                elementsByFamilyString[fm.Symbol.FamilyName] = stringValue;
//                            }
//                            stringValue.Add(fm.Symbol.FamilyName);
//                        }

//                        // isGrouped
//                        if (e?.GroupId is not null)
//                        {
//                            isGrouped = true;
//                            groupId = e.GroupId.ToString();
//                        }

//                        // isModifiable
//                        if (e.IsModifiable) isModifiable = true;

//                        // isViewSpecific
//                        if (!e.ViewSpecific)
//                        {
//                            familyName = fm?.Symbol.Family.Name;
//                        }
//                        else
//                        {
//                            isViewSpecific = true;
//                            ownerViewId = e.OwnerViewId.ToString();
//                        }

//                        // isBuiltInCategory + builtInCategory
//                        if (e.Category is not null && e.Category.BuiltInCategory is not BuiltInCategory.INVALID)
//                        {
//                            isBuiltInCategory = true;
//                            category = e.Category.Name;
//                        }

//                        if (e.WorksetId is not null) worksetId = e.WorksetId.ToString();

//                        if (e.HasPhases())
//                        {
//                            if (e.CreatedPhaseId is not null) createdPhaseId = e.CreatedPhaseId.ToString();
//                            if (e.DemolishedPhaseId is not null) demolishedPhaseId = e.DemolishedPhaseId.ToString();
//                        }

//                        if (e.DesignOption is not null) designOption = e.DesignOption.Name;

//                        if (e.Document is not null) docOwner = doc.CreationGUID.ToString();

//                        isPinned = e.Pinned;

//                        if (doc.IsWorkshared)
//                        {
//                            isWorkshared = true;
//                            workshareId = doc.WorksharingCentralGUID.ToString();
//                        }

//                        if (worksetParam != null)
//                        {
//                            string worksetName = worksetParam.AsValueString();

//                            if (worksetElementCount.TryGetValue(worksetName, out int value))
//                            {
//                                worksetElementCount[worksetName] = ++value;
//                            }
//                            else
//                            {
//                                worksetElementCount[worksetName] = 1;
//                            }
//                        }

//                        if (e.LevelId is not null) levelId = e.LevelId.ToString();

//                        // create element info
//                        List<ParameterInformation?>? parameters = [];
//                        parameters.AddRange(from p in e.GetOrderedParameters()
//                                            select p.GetParameterFromElement(doc));
//                        Dictionary<string, string> processed = [];
//                        foreach (var p in parameters)
//                        {
//                            try
//                            {
//                                processed.Add("parameterGuid", p.Value.parameterGuid);
//                                processed.Add("documentOwner", p.Value.documentOwner);
//                                processed.Add("storageType", p.Value.storageType.ToString());
//                                processed.Add("hasValue", p.Value.hasValue.ToString());
////                                processed.Add("value", p.Value.value.ToString() ?? string.Empty);
////                                processed.Add("parameterIdValue", p.Value.parameterIdValue.ToString());
////                                processed.Add("isUserModifiable", p.Value.ToString());
////                            }
////                            catch
////                            {
////                                continue;
////                            }
////                        }

////                        individualElementInfo.Push(new ElementInformation
////                        {
////                            idValue = e.Id.Value,
////                            uniqueElementId = e.UniqueId,
////                            elementVersionId = e.VersionGuid.ToString(),
////                            familyName = familyName,
////                            category = builtInCategory,
////                            builtInCategory = builtInCategory,
////                            workset = workset,
////                            views = views,
////                            designOption = designOption,
////                            documentOwner = docOwner,
////                            ownerViewId = ownerViewId,
////                            worksetId = worksetId,
////                            levelId = levelId,
////                            createdPhaseId = createdPhaseId,
////                            demolishedPhaseId = demolishedPhaseId,
////                            groupId = groupId,
////                            workshareId = workshareId,
////                            isGrouped = isGrouped,
////                            isModifiable = isModifiable,
////                            isViewSpecific = isViewSpecific,
////                            isBuiltInCategory = isBuiltInCategory,
////                            isAnnotative = isAnnotative,
////                            isModel = isModel,
////                            isPinned = isPinned,
////                            isWorkshared = isWorkshared,
////                            Parameters = processed
////                        });

////                        // filter element for indicators
////                        switch (e)
////                        {
////                            case View:
////                                View? v = e as View;

////                                if (v is not null && !v.IsTemplate)
////                                {
////                                    viewsInsideDocument.Add(v);
////                                }
////                                builtInCategory = e?.Category?.BuiltInCategory.ToString();
////                                break;
////                            case Viewport:
////                                Viewport? vp = e as Viewport;
////                                if (vp is not null) viewports.Add(vp);
////                                builtInCategory = e?.Category?.BuiltInCategory.ToString();
////                                break;
////                            case Group:
////                                Group? g = e as Group;
////                                if (g is not null)
////                                {
////                                    if (g.Category.Name == "Detail Groups")
////                                    {
////                                        detailGroups.Add(g);
////                                    }
////                                    else
////                                    {
////                                        modelGroups.Add(g);
////                                    }
////                                }
////                                builtInCategory = e?.Category?.BuiltInCategory.ToString();
////                                break;
////                            case DesignOption:
////                                DesignOption? option = e as DesignOption;
////                                if (option is not null) designOptions.Add(option);
////                                builtInCategory = e?.Category?.BuiltInCategory.ToString();
////                                break;
////                            case Level:
////                                Level? l = e as Level;
////                                if (l is not null) levels.Add(l);
////                                builtInCategory = e?.Category?.BuiltInCategory.ToString();
////                                break;
////                            case Grid:
////                                Grid? gr = e as Grid;
////                                if (gr is not null) grids.Add(gr);
////                                builtInCategory = e?.Category?.BuiltInCategory.ToString();
////                                break;
////                            case GraphicsStyle:
////                                GraphicsStyle? graphicsStyle = e as GraphicsStyle;
////                                if (graphicsStyle is not null)
////                                {
////                                    Category c = graphicsStyle.GraphicsStyleCategory;
////                                    builtInCategory = e?.Category?.BuiltInCategory.ToString();

////                                    if (c is not null && c.IsCuttable is not false && c.CategoryType == CategoryType.Annotation)
////                                    {
////                                        nonNativeStyles.Add(graphicsStyle);
////                                    }
////                                }
////                                break;
////                            case FamilyInstance:
////                                if (fm is not null)
////                                {
////                                    if (fm.Mirrored) isFlipped.Add(e);
////                                    builtInCategory = e?.Category?.BuiltInCategory.ToString();
////                                }
////                                break;
////                            default:
////                                switch (e?.Category?.BuiltInCategory)
////                                {
////                                    case BuiltInCategory.OST_Rooms:
////                                        Room? room = e as Room;
////                                        if (room is not null)
////                                        {
////                                            IList<IList<BoundarySegment>> boundarySegments = room.GetBoundarySegments(new SpatialElementBoundaryOptions());

////                                            if (boundarySegments == null || boundarySegments.Count == 0)
////                                            {
////                                                unenclosedRoom.Add(room);
////                                            }
////                                        }
////                                        builtInCategory = e?.Category?.BuiltInCategory.ToString();

////                                        break;
////                                    case BuiltInCategory.OST_DuctCurves:
////                                        if (e is Duct duct)
////                                        {
////                                            bool isUnconnected = false;

////                                            ConnectorSet connectors = duct.ConnectorManager.Connectors;
////                                            foreach (Connector connector in connectors)
////                                            {
////                                                if (!connector.IsConnected)
////                                                {
////                                                    isUnconnected = true;
////                                                    break;
////                                                }
////                                            }

////                                            if (isUnconnected)
////                                            {
////                                                unconnectedDucts.Add(duct);
////                                            }
////                                        }
////                                        builtInCategory = e?.Category?.BuiltInCategory.ToString();
////                                        break;
////                                    case BuiltInCategory.OST_PipeCurves:
////                                        if (e is Pipe pipe)
////                                        {
////                                            bool isUnconnected = false;

////                                            ConnectorSet connectors = pipe.ConnectorManager.Connectors;
////                                            foreach (Connector connector in connectors)
////                                            {
////                                                if (!connector.IsConnected)
////                                                {
////                                                    isUnconnected = true;
////                                                    break;
////                                                }
////                                            }

////                                            if (isUnconnected)
////                                            {
////                                                unconnectedPipes.Add(pipe);
////                                            }
////                                        }
////                                        builtInCategory = e?.Category?.BuiltInCategory.ToString();
////                                        break;
////                                    case BuiltInCategory.OST_ElectricalFixtures:
////                                        MEPModel mepModel = ((FamilyInstance)e).MEPModel;
////                                        if (mepModel != null)
////                                        {
////                                            ConnectorSet connectors = mepModel.ConnectorManager.Connectors;
////                                            foreach (Connector connector in connectors)
////                                            {
////                                                if (!connector.IsConnected)
////                                                {
////                                                    unconnectedElectrical.Add(connector);
////                                                }
////                                            }
////                                        }
////                                        builtInCategory = e?.Category?.BuiltInCategory.ToString();
////                                        break;
////                                    default:
////                                        builtInCategory = e?.Category?.BuiltInCategory.ToString();
////                                        break;
////                                }

////                                //TODO: check for unused
////                                //Debug.Print(e.Name + "" + typeof(Element).Name);
////                                break;
////                        }

////                        switch (e?.Category?.CategoryType)
////                        {
////                            case CategoryType.Model:
////                                isAnnotative = false;
////                                isModel = true;
////                                builtInCategory = CategoryType.Model.ToString();
////                                break;
////                            case CategoryType.Annotation:
////                                isAnnotative = true;
////                                isModel = false;
////                                builtInCategory = CategoryType.Annotation.ToString();
////                                annotativeElements.Add(e);
////                                break;
////                            case CategoryType.Invalid:
////                            case CategoryType.Internal:
////                            case CategoryType.AnalyticalModel:
////                            default:
////                                break;
////                        }

////                    }

////                    // view not in sheet. needs to be done after all are done.
////                    HashSet<ElementId> viewsOnSheets = [.. viewports.Select(vp => (vp as Viewport).ViewId)];
////                    foreach (Element viewElement in viewsInsideDocument)
////                    {
////                        if (viewElement is View view && !view.IsTemplate && !viewsOnSheets.Contains(view.Id))
////                        {
////                            notInSheets.Add(view);
////                        }
////                    }

////                    foreach (ElementId reference in ExternalFileUtils.GetAllExternalFileReferences(doc))
////                    {
////                        Element ext = doc.GetElement(reference);
////                        externalRefs.Add((ext.GetExternalFileReference().ExternalFileReferenceType, ext.GetExternalFileReference()));
////                    }

////                    Dictionary<string, object> results = new()
////                    {
////                        { "viewsInsideDocument", viewsInsideDocument.Count },
////                        { "notInSheets", notInSheets.Count },
////                        { "annotativeElements", annotativeElements.Count },
////                        { "externalRefs", externalRefs.Count },
////                        { "modelGroups", modelGroups.Count },
////                        { "detailGroups", detailGroups.Count },
////                        { "designOptions", designOptions.Count },
////                        { "levels", levels.Count },
////                        { "grids", grids.Count },
////                        { "warns", warns.Count },
////                        { "unenclosedRoom", unenclosedRoom.Count },
////                        { "viewports", viewports.Count },
////                        { "unconnectedDucts", unconnectedDucts.Count },
////                        { "unconnectedPipes", unconnectedPipes.Count },
////                        { "unconnectedElectrical", unconnectedElectrical.Count },
////                        { "nonNativeStyles", nonNativeStyles.Count },
////                        { "isFlipped", isFlipped.Count },
////                        { "worksetElementCount", worksetElementCount.Count }
////                    };

////                    Dictionary<string, double> elementCount = [];
////                    foreach (KeyValuePair<Family, List<Element?>> ee in getElementsByFamily)
////                    {
////                        elementCount.Add(ee.Key.Name, ee.Value.Count);
////                    }


////                    Dictionary<ElementInformation, List<ParameterInformation?>>? introspectionInfo(IEnumerable<Element> list)
////                    {
////                        Dictionary<ElementInformation, List<ParameterInformation?>>? results = [];
////                        List<ParameterInformation?> parameters = [];

////                        foreach (var e in list)
////                        {
////                            ElementInformation el = e.GetElementInformation(doc);

////                            foreach (var pl in e.GetOrderedParameters())
////                            {
////                                ParameterInformation? pi = pl.GetParameterFromElement(doc);
////                                parameters.Add(pi);
////                            }
////                            results.Add(el, parameters);
////                        }
////                        return results;
////                    }

////                    Dictionary<string, string>? refs = [];
////                    foreach (var r in externalRefs)
////                    {
////                        ExternalFileReferenceType rt = r.Item1;
////                        ExternalFileReference fr = r.Item2;

////                        refs.Add(rt.ToString(), fr.PathType.ToString());
////                    }

////                    ModelIntrospection m = new()
////                    {
////                        elementCountTotal = collector.Count,
////                        familiyElementCount = elementsByFamilyString,
////                        documentVersion = doc.ProjectInformation.VersionGuid.ToString(),
////                        fileSize = new FileInfo(doc.PathName).Length,
////                        warnings = [.. doc.GetWarnings().Select(x => x.GetDescriptionText())],
////                        elementCount = elementCount,
////                        viewsInsideDocument = introspectionInfo(viewsInsideDocument),
////                        notInSheets = introspectionInfo(notInSheets),
////                        annotativeElements = introspectionInfo(annotativeElements),
////                        externalRefs = refs,
////                        modelGroups = introspectionInfo(modelGroups),
////                        detailGroups = introspectionInfo(detailGroups),
////                        designOptions = introspectionInfo(designOptions),
////                        levels = introspectionInfo(levels),
////                        grids = introspectionInfo(grids),
////                        unenclosedRoom = introspectionInfo(unenclosedRoom),
////                        viewports = introspectionInfo(viewports),
////                        unconnectedDucts = introspectionInfo(unconnectedDucts),
////                        unconnectedPipes = introspectionInfo(unconnectedPipes),
////                        unconnectedElectrical = introspectionInfo((IEnumerable<Element>)unconnectedElectrical),
////                        nonNativeStyles = introspectionInfo(nonNativeStyles),
////                        isFlipped = introspectionInfo(isFlipped),
////                        worksetElementCount = worksetElementCount
////                    };

////                    return new Prey(m);
////                }
////                catch
////                {
////                    failCounter++;
////                    continue;
////                }
////            }
////            Debug.Print(failCounter.ToString());
////            return new Prey(JsonSerializer.Serialize(new KeyValuePair<string, int>(key: "failCount", value: failCounter)));
////        }


////        public override bool Execute()
////        {
////            SendCatchToCallback(ProcessInfo());
////            return true;
////        }
////    }
////}
