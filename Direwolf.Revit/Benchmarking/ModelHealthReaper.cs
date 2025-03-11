using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
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
            public bool? IsGrouped { get; init; }
            public bool? IsModifiable { get; init; }
            public bool? IsViewSpecific { get; init; }
            public bool? IsBuiltInCategory { get; init; }
            public bool? IsAnnotative { get; init; }
            public bool? IsModel { get; init; }
            public bool? IsInternal { get; init; }
            public bool? IsPinned { get; init; }
            public Dictionary<string, string>? Parameters { get; init; }
        }



        private void Process1()
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

            foreach (Element e in collector)
            {
                Parameter worksetParam = e.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);

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

                switch (e)
                {
                    case (View):
                        View? v = e as View;

                        if (v is not null && !v.IsTemplate)
                        {
                            viewsInsideDocument.Add(v);
                        }
                        break;
                    case (Viewport):
                        Viewport? vp = e as Viewport;
                        if (vp is not null) viewports.Add(vp);
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
                        break;
                    case (DesignOption):
                        DesignOption? option = e as DesignOption;
                        if (option is not null) designOptions.Add(option);
                        break;
                    case (Level):
                        Level? l = e as Level;
                        if (l is not null) levels.Add(l);
                        break;
                    case (Grid):
                        Grid? gr = e as Grid;
                        if (gr is not null) grids.Add(gr);
                        break;
                    case (GraphicsStyle):
                        GraphicsStyle? graphicsStyle = e as GraphicsStyle;
                        if (graphicsStyle is not null)
                        {
                            Category category = graphicsStyle.GraphicsStyleCategory;

                            if (category is not null && category.IsCuttable is not false && category.CategoryType == CategoryType.Annotation)
                            {
                                nonNativeStyles.Add(graphicsStyle);
                            }
                        }
                        break;
                    case (FamilyInstance):
                        FamilyInstance? fi = e as FamilyInstance;
                        if (fi is not null)
                        {
                            if (fi.Mirrored) isFlipped.Add(e);
                        }
                        break;
                    default:
                        if (e.Category != null)
                        {
                            switch (e.Category.CategoryType)
                            {
                                case CategoryType.Annotation:
                                    annotativeElements.Add(e);
                                    break;
                                default:
                                    break;
                            }

                            switch (e.Category.BuiltInCategory)
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
                                    break;
                            }
                        }

                        //TODO: check for unused
                        //Debug.Print(e.Name + "" + typeof(Element).Name);
                        break;
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
        }


        public override bool Execute()
        {
            return true;
        }


    }
}
