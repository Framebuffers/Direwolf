// using System.Diagnostics;
// using Autodesk.Revit.DB;
// using Autodesk.Revit.DB.Architecture;
// using Autodesk.Revit.DB.Mechanical;
// using Autodesk.Revit.DB.Plumbing;
// using Direwolf.Contracts;
// using Direwolf.Primitives;
// using Direwolf.Revit.Extensions;
// using ElementType = Autodesk.Revit.DB.ElementType;
//
// namespace Direwolf.Revit.Howls;
//
// public record ModelSnapshot : Definitions.Primitives.RevitHowl
// {
//     private readonly List<Element> _annotativeElements = [];
//     private readonly List<DesignOption> _designOptions = [];
//     private readonly List<Group> _detailGroups = [];
//     private readonly List<(ExternalFileReferenceType, ExternalFileReference)> _externalRefs = [];
//     private readonly List<Grid> _grids = [];
//     private readonly List<Element> _isFlipped = [];
//     private readonly List<Level> _levels = [];
//     private readonly List<Group> _modelGroups = [];
//     private readonly List<GraphicsStyle> _nonNativeStyles = [];
//     private readonly List<View> _notInSheets = [];
//     private readonly List<Duct> _unconnectedDucts = [];
//     private readonly List<Connector> _unconnectedElectrical = [];
//     private readonly List<Pipe> _unconnectedPipes = [];
//     private readonly List<Room> _unenclosedRoom = [];
//     private readonly List<Viewport> _viewports = [];
//
//     // These are all categories for which information has to be extracted.
//     private readonly List<View> _viewsInsideDocument = [];
//     private readonly List<FailureMessage> _warns = [];
//     private readonly Dictionary<string, int> _worksetElementCount = [];
//     private Stack<Dictionary<string, object>> _individualElementInfo = [];
//
//     private Stopwatch s { get; } = new();
//
//     public override void ExecuteHunt()
//     {
//         ProcessInfo();
//     }
//
//     private IWolfpack ProcessInfo()
//     {
//         s.Start();
//         var doc = Document;
//         _warns.AddRange(Document.GetWarnings());
//         ICollection<Element> collector =
//             [.. new FilteredElementCollector(Document).WhereElementIsNotElementType()];
//
//         foreach (var element in collector)
//         {
//             var e = doc.GetElement(element.Id);
//             if ((e is not null && e.IsValidObject && e.Category is not null &&
//                  e.Category.CategoryType is not CategoryType.Invalid) ||
//                 e?.Category?.CategoryType is not CategoryType.Internal)
//             {
//                 var worksetParam = e?.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);
//
//                 var familyName = string.Empty;
//                 var category = string.Empty;
//                 var builtInCategory = string.Empty;
//                 var workset = string.Empty;
//                 string[]? views = [];
//                 var designOption = string.Empty;
//                 var docOwner = string.Empty;
//                 var ownerViewId = string.Empty;
//                 var worksetId = string.Empty;
//                 var createdPhaseId = string.Empty;
//                 var demolishedPhaseId = string.Empty;
//                 var groupId = string.Empty;
//                 var workshareId = string.Empty;
//                 var levelId = string.Empty;
//                 bool? isGrouped = false;
//                 bool? isModifiable = false;
//                 bool? isViewSpecific = false;
//                 bool? isBuiltInCategory = false;
//                 bool? isAnnotative = false;
//                 bool? isModel = false;
//                 bool? isPinned = false;
//                 bool? isWorkshared = false;
//
//                 var fm = e as FamilyInstance;
//
//                 // isGrouped
//                 if (e?.GroupId is not null)
//                 {
//                     isGrouped = true;
//                     groupId = e.GroupId.ToString();
//                 }
//
//                 // isModifiable
//                 if (e.IsModifiable) isModifiable = true;
//
//                 // isViewSpecific
//                 if (!e.ViewSpecific)
//                 {
//                     familyName = fm?.Symbol.Family.Name;
//                 }
//                 else
//                 {
//                     isViewSpecific = true;
//                     ownerViewId = e.OwnerViewId.ToString();
//                 }
//
//                 // isBuiltInCategory + builtInCategory
//                 if (e.Category is not null && e.Category.BuiltInCategory is not BuiltInCategory.INVALID)
//                 {
//                     isBuiltInCategory = true;
//                     category = e.Category.Name;
//                 }
//
//                 if (e.WorksetId is not null) worksetId = e.WorksetId.ToString();
//
//                 if (e.HasPhases())
//                 {
//                     if (e.CreatedPhaseId is not null) createdPhaseId = e.CreatedPhaseId.ToString();
//                     if (e.DemolishedPhaseId is not null) demolishedPhaseId = e.DemolishedPhaseId.ToString();
//                 }
//
//                 if (e.DesignOption is not null) designOption = e.DesignOption.Name;
//
//                 if (e.Document is not null) docOwner = doc.CreationGUID.ToString();
//
//                 isPinned = e.Pinned;
//
//                 if (doc.IsWorkshared)
//                 {
//                     isWorkshared = true;
//                     workshareId = doc.WorksharingCentralGUID.ToString();
//                 }
//
//                 if (worksetParam != null)
//                 {
//                     var worksetName = worksetParam.AsValueString();
//
//                     if (_worksetElementCount.TryGetValue(worksetName, out var value))
//                         _worksetElementCount[worksetName] = ++value;
//                     else
//                         _worksetElementCount[worksetName] = 1;
//                 }
//
//                 if (e.LevelId is not null) levelId = e.LevelId.ToString();
//
//                 switch (e)
//                 {
//                     case View:
//                         var v = e as View;
//
//                         if (v is not null && !v.IsTemplate) _viewsInsideDocument.Add(v);
//                         builtInCategory = e?.Category?.BuiltInCategory.ToString();
//                         break;
//                     case Viewport:
//                         var vp = e as Viewport;
//                         if (vp is not null) _viewports.Add(vp);
//                         builtInCategory = e?.Category?.BuiltInCategory.ToString();
//                         break;
//                     case Group:
//                         var g = e as Group;
//                         if (g is not null)
//                         {
//                             if (g.Category.Name == "Detail Groups")
//                                 _detailGroups.Add(g);
//                             else
//                                 _modelGroups.Add(g);
//                         }
//
//                         builtInCategory = e?.Category?.BuiltInCategory.ToString();
//                         break;
//                     case DesignOption:
//                         var option = e as DesignOption;
//                         if (option is not null) _designOptions.Add(option);
//                         builtInCategory = e?.Category?.BuiltInCategory.ToString();
//                         break;
//                     case Level:
//                         var l = e as Level;
//                         if (l is not null) _levels.Add(l);
//                         builtInCategory = e?.Category?.BuiltInCategory.ToString();
//                         break;
//                     case Grid:
//                         var gr = e as Grid;
//                         if (gr is not null) _grids.Add(gr);
//                         builtInCategory = e?.Category?.BuiltInCategory.ToString();
//                         break;
//                     case GraphicsStyle:
//                         var graphicsStyle = e as GraphicsStyle;
//                         if (graphicsStyle is not null)
//                         {
//                             var c = graphicsStyle.GraphicsStyleCategory;
//                             builtInCategory = e?.Category?.BuiltInCategory.ToString();
//
//                             if (c is not null && c.IsCuttable is not false && c.CategoryType == CategoryType.Annotation)
//                                 _nonNativeStyles.Add(graphicsStyle);
//                         }
//
//                         break;
//                     case FamilyInstance:
//                         if (fm is not null)
//                         {
//                             if (fm.Mirrored) _isFlipped.Add(e);
//                             builtInCategory = e?.Category?.BuiltInCategory.ToString();
//                         }
//
//                         break;
//                     default:
//                         switch (e?.Category?.BuiltInCategory)
//                         {
//                             case BuiltInCategory.OST_Rooms:
//                                 var room = e as Room;
//                                 if (room is not null)
//                                 {
//                                     IList<IList<BoundarySegment>> boundarySegments =
//                                         room.GetBoundarySegments(new SpatialElementBoundaryOptions());
//
//                                     if (boundarySegments == null || boundarySegments.Count == 0)
//                                         _unenclosedRoom.Add(room);
//                                 }
//
//                                 builtInCategory = e?.Category?.BuiltInCategory.ToString();
//
//                                 break;
//                             case BuiltInCategory.OST_DuctCurves:
//                                 if (e is Duct duct)
//                                 {
//                                     var isUnconnected = false;
//
//                                     var connectors = duct.ConnectorManager.Connectors;
//                                     foreach (Connector connector in connectors)
//                                         if (!connector.IsConnected)
//                                         {
//                                             isUnconnected = true;
//                                             break;
//                                         }
//
//                                     if (isUnconnected) _unconnectedDucts.Add(duct);
//                                 }
//
//                                 builtInCategory = e?.Category?.BuiltInCategory.ToString();
//                                 break;
//                             case BuiltInCategory.OST_PipeCurves:
//                                 if (e is Pipe pipe)
//                                 {
//                                     var isUnconnected = false;
//
//                                     var connectors = pipe.ConnectorManager.Connectors;
//                                     foreach (Connector connector in connectors)
//                                         if (!connector.IsConnected)
//                                         {
//                                             isUnconnected = true;
//                                             break;
//                                         }
//
//                                     if (isUnconnected) _unconnectedPipes.Add(pipe);
//                                 }
//
//                                 builtInCategory = e?.Category?.BuiltInCategory.ToString();
//                                 break;
//                             case BuiltInCategory.OST_ElectricalFixtures:
//                                 var mepModel = ((FamilyInstance)e).MEPModel;
//                                 if (mepModel != null)
//                                 {
//                                     var connectors = mepModel.ConnectorManager.Connectors;
//                                     foreach (Connector connector in connectors)
//                                         if (!connector.IsConnected)
//                                             _unconnectedElectrical.Add(connector);
//                                 }
//
//                                 builtInCategory = e?.Category?.BuiltInCategory.ToString();
//                                 break;
//                             default:
//                                 builtInCategory = e?.Category?.BuiltInCategory.ToString();
//                                 break;
//                         }
//
//                         //TODO: check for unused
//                         //Debug.Print(e.Name + "" + typeof(Element).Name);
//                         break;
//                 }
//
//                 switch (e?.Category?.CategoryType)
//                 {
//                     case CategoryType.Model:
//                         isAnnotative = false;
//                         isModel = true;
//                         builtInCategory = CategoryType.Model.ToString();
//                         break;
//                     case CategoryType.Annotation:
//                         isAnnotative = true;
//                         isModel = false;
//                         builtInCategory = CategoryType.Annotation.ToString();
//                         _annotativeElements.Add(e);
//                         break;
//                     case CategoryType.Invalid:
//                     case CategoryType.Internal:
//                     case CategoryType.AnalyticalModel:
//                     default:
//                         break;
//                 }
//
//                 /**
//                  * This code can serialize the entire model to a JSON file. However, it will export a several-hundred MB JSON file. It *will* crash the database.
//                  *
//                  * In case you *need* to track every single Element, you'd need to implement a different database solution.
//                  *
//                 Dictionary<string, object>? elementInfo = new()
//                 {
//                     ["id"] = e.Id.Value,
//                     ["uniqueId"] = e.UniqueId,
//                     ["familyName"] = GetElementType(e)?.FamilyName ?? string.Empty,
//                     ["elementName"] = e?.Name ?? string.Empty,
//                 };
//
//                 List<Dictionary<string, object>> result =
//                 [
//                     elementInfo,
//                             new Dictionary<string, object>() { ["parameters"] = GetParameters(e) },
//                             GetCategory(e),
//                             new Dictionary<string, object>() { ["materials"] = e.GetMaterialIds(true)},
//                         ];
//
//                 SendWolfpackBack(new Prey(result));
//                 */
//             }
//         }
//
//         // view not in sheet. needs to be done after all are done.
//         HashSet<ElementId> viewsOnSheets = [.. _viewports.Select(vp => vp.ViewId)];
//         foreach (Element viewElement in _viewsInsideDocument)
//             if (viewElement is View view && !view.IsTemplate && !viewsOnSheets.Contains(view.Id))
//                 _notInSheets.Add(view);
//
//         foreach (var reference in ExternalFileUtils.GetAllExternalFileReferences(doc))
//         {
//             var ext = doc.GetElement(reference);
//             _externalRefs.Add((ext.GetExternalFileReference().ExternalFileReferenceType,
//                 ext.GetExternalFileReference()));
//         }
//
//         Dictionary<string, object> results = new()
//         {
//             { "viewsInsideDocument", _viewsInsideDocument.Count },
//             { "notInSheets", _notInSheets.Count },
//             { "annotativeElements", _annotativeElements.Count },
//             { "externalRefs", _externalRefs.Count },
//             { "modelGroups", _modelGroups.Count },
//             { "detailGroups", _detailGroups.Count },
//             { "designOptions", _designOptions.Count },
//             { "levels", _levels.Count },
//             { "grids", _grids.Count },
//             { "warns", _warns.Count },
//             { "unenclosedRoom", _unenclosedRoom.Count },
//             { "viewports", _viewports.Count },
//             { "unconnectedDucts", _unconnectedDucts.Count },
//             { "unconnectedPipes", _unconnectedPipes.Count },
//             { "unconnectedElectrical", _unconnectedElectrical.Count },
//             { "nonNativeStyles", _nonNativeStyles.Count },
//             { "isFlipped", _isFlipped.Count },
//             { "worksetElementCount", _worksetElementCount.Count }
//         };
//         s.Stop();
//         return Wolfpack.New("ModelSnapshot", results, s.ElapsedMilliseconds);
//         //SendWolfpackBack(new Prey(doc._GetInstancesPerFamily()));
//         // SendCatchToCallback(new Prey(results));
//     }
//
//     private static Dictionary<string, object>? GetCategory(Element e)
//     {
//         Dictionary<string, object> categoryInfo = [];
//
//         if (e is not null && e.Category is not null)
//         {
//             categoryInfo.TryAdd("categoryId", e.Category.Id.Value);
//             categoryInfo.TryAdd("categoryUniqueId", e.UniqueId ?? Guid.Empty.ToString());
//             categoryInfo.TryAdd("categoryName", e.Category.Name ?? string.Empty);
//             categoryInfo.TryAdd("builtInCategory", e.Category.BuiltInCategory.ToString() ?? string.Empty);
//             categoryInfo.TryAdd("categoryType", e.Category.CategoryType.ToString() ?? string.Empty);
//             categoryInfo.TryAdd("hasMaterialQuantities", e.Category.HasMaterialQuantities);
//         }
//
//         return categoryInfo;
//     }
//
//     private static List<Dictionary<string, object>>? GetParameters(Element e)
//     {
//         List<Dictionary<string, object>>? results = [];
//         if (e is not null && e.GetOrderedParameters() is not null)
//         {
//             IList<Parameter>? b = e.GetOrderedParameters();
//             if (e?.Category is not null)
//                 results.AddRange(from p in b
//                     select p.GetParameterValue());
//             return results;
//         }
//
//         return [];
//     }
//
//     private static ElementType? GetElementType(Element e)
//     {
//         var type = e?.GetTypeId();
//         if (type is not null) return e?.Document?.GetElement(type) as ElementType;
//         return null;
//     }
// }

