using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Direwolf.Definitions;
using Direwolf.Revit.Howlers;
using Revit.Async;
using System.Diagnostics;
using System.Text.Json;

namespace Direwolf.Revit.Benchmarking
{
    [Transaction(TransactionMode.Manual)]
    public class BaseCommandSanityCheck : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            using StringWriter s = new();
            Stopwatch st = new();
            st.Start();
            Debug.Print(JsonSerializer.Serialize(DateTime.Now));
            string lastCommand = string.Empty;

            // externalfilereferences = revit files

            //using FilteredElementCollector c = new(doc);
            //ICollection<Element> all = c
            //    .WhereElementIsNotElementType()
            //    .WhereElementIsViewIndependent()
            //    .ToList();

            //ICollection<RevitLinkInstance> rvtlink = c
            //    .WhereElementIsNotElementType()
            //    .OfClass(typeof(RevitLinkInstance))
            //    .Cast<RevitLinkInstance>()
            //    .ToList();

            //ICollection<ImageInstance> raster = c
            //    .WhereElementIsNotElementType()
            //    .OfClass(typeof(ImageInstance))
            //    .Cast<ImageInstance>()
            //    .ToList();

            ////ICollection<ExternalResourceReference> ext = c
            ////    .OfClass(typeof(ExternalResourceReference))
            ////    .Cast<ExternalResourceReference>()
            ////    .ToList();




            //ExternalResourceType x = ExternalResourceTypes.BuiltInExternalResourceTypes.CADLink;

            //Dictionary<string, object> counts = new()
            //{
            //    ["rvtLinks"] = rvtlink.Select(x => x.Name),
            //    ["raster"] = rvtlink.Select(x => x.Name)
            //};

            //Debug.Print(JsonSerializer.Serialize(counts, new JsonSerializerOptions() { WriteIndented = true }));

            ////Debug.Print(ext.Count.ToString());


            //Dictionary<string, object> d = [];

            ////doc.ProjectInformation.Get
            //foreach (Element e in all)
            //{
            //    if (e.IsExternalFileReference())
            //    {
            //        var r = e.GetExternalFileReference();
            //        var referenceData = new Dictionary<string, object>
            //        {
            //            ["Name"] = e.Name,
            //            ["LinkedFileStatus"] = r.GetLinkedFileStatus().ToString(),
            //            ["Path"] = r.GetPath().Empty ? r.GetAbsolutePath().CloudPath : doc.PathName,
            //            ["ReferenceType"] = r.ExternalFileReferenceType.ToString(),
            //            ["PathType"] = r.PathType.ToString()
            //        };

            //        d.Add($"{e.UniqueId}", referenceData);

            //    }
            //}
            //Debug.Print(JsonSerializer.Serialize(d, new JsonSerializerOptions() { WriteIndented = true }));
            ////Debug.Print($"rvtLink={rvtlink.Count}, cadLink={cadlink.Count}");



            Dictionary<string, object> individualInfo = [];
            try
            {


                RevitTask.Initialize(commandData.Application);
                RevitHowler rh = new();
                rh.CreateWolf(new Wolf(), new GetModelSnapshot(doc));

                Direwolf dw = new(commandData.Application);
                dw.QueueHowler(rh);
                dw.HuntAsync("Model Health");

                st.Stop();
                Debug.WriteLine($"TimeTakenSync: {st.Elapsed.TotalSeconds}.");
                Debug.Print(JsonSerializer.Serialize(DateTime.Now));
                dw.WriteQueriesToJson();
            }
            catch
            {
                return Result.Failed;
            }

            //try
            //{
            //    s.WriteLine(GetViews(doc));
            //    lastCommand = "GetViews";
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetAnnotativeElements(doc));
            //    lastCommand = "GetAnnotativeElements";
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetImportedInstances(doc));
            //    lastCommand = "GetImportedInstances";
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetDWGFiles(doc));
            //    lastCommand = "GetDWGFiles";
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetSKPFiles(doc));
            //    lastCommand = "GetSKPFiles";
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetModelGroups(doc));
            //    lastCommand = "GetModelGroups";
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetDetailGroups(doc));
            //    lastCommand = "GetDetailGroups";
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetDesignOptions(doc));
            //    lastCommand = "GetDesignOptions";
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetLevelCount(doc));
            //    lastCommand = "GetLevelCount";
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetGridLineCount(doc));
            //    lastCommand = "GetGridLineCount";
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetErrorsAndWarnings(doc));
            //    lastCommand = "GetErrorsAndWarnings";
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetUnusedFamilies(doc));
            //    lastCommand = "GetUnusedFamilies";

            //    s.WriteLine(GetUnenclosedRooms(doc));
            //    lastCommand = "GetUnenclosedRooms";
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetDuplicateElements(doc));
            //    lastCommand = "GetDuplicateElements";
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetViewsNotOnSheets(doc));
            //    lastCommand = "GetViewsNotOnSheets";
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetUnconnectedDucts(doc));
            //    lastCommand = "GetUnconnectedDucts";
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetUnconnectedPipes(doc));
            //    lastCommand = "GetUnconnectedPipes";
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetUnconnectedElectrical(doc));
            //    lastCommand = "GetUnconnectedElectrical";
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetNonNativeObjectStyles(doc));
            //    lastCommand = "GetNonNativeObjectStyles";
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetMirroredObjects(doc));
            //    lastCommand = "GetMirroredObjects";
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetElementsByWorkset(doc));
            //    lastCommand = "GetElementsByWorkset";
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetFamilyCount(doc));
            //    lastCommand = "GetFamilyCount";
            //    s.WriteLine(lastCommand);
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetFamilyCountBySize(doc));
            //    lastCommand = "GetFamilyCountBySize";
            //    s.WriteLine(lastCommand);
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetFamliesWithMostTypes(doc));
            //    lastCommand = "GetFamliesWithMostTypes";
            //    s.WriteLine(lastCommand);
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetFamiliesWithMostTypes(doc));
            //    lastCommand = "GetFamiliesWithMostTypes";
            //    s.WriteLine(lastCommand);
            //    Debug.Print(s.ToString());

            //    s.WriteLine(TotalSizeOfFamiliesByMB(doc));
            //    lastCommand = "TotalSizeByMB";
            //    s.WriteLine(lastCommand);
            //    Debug.Print(s.ToString());

            //    s.WriteLine(GetElementsByWorkset(doc));
            //    lastCommand = "Elements by Workset";
            //    s.WriteLine(lastCommand);
            //    Debug.Print(s.ToString());

            //}
            //catch
            //{
            //    Debug.Print(lastCommand);
            //}

            // Get a very generic collector
            //ICollection<Element> collector = [.. new FilteredElementCollector(doc).WhereElementIsNotElementType()];


            // GetViewsInsideDocument
            //List<View> viewsInsideDocument = [];
            //foreach (Element e in collector)
            //{
            //    try
            //    {
            //        switch (e)
            //        {
            //            case (View):
            //                Debug.Print(e.Name + "\t" + typeof(View).Name);
            //                break;
            //            case (ImportInstance):
            //                Debug.Print(e.Name + "\t" + typeof(ImportInstance).Name);
            //                break;
            //            case (Group):
            //                Debug.Print(e.Name + "\t" + typeof(Group).Name);
            //                break;
            //            case (DesignOption):
            //                Debug.Print(e.Name + "\t" + typeof(DesignOption).Name);
            //                break;
            //            case (Level):
            //                Debug.Print(e.Name + "\t" + typeof(Level).Name);
            //                break;
            //            case (Grid):
            //                Debug.Print(e.Name + "\t" + typeof(Grid).Name);
            //                break;
            //            case (Family):
            //                Debug.Print(e.Name + "\t" + typeof(Family).Name);
            //                break;
            //            case (GraphicsStyle):
            //                Debug.Print(e.Name + "\t" + typeof(GraphicsStyle).Name);
            //                break;
            //            case (FamilyInstance):
            //                Debug.Print(e.Name + "\t" + typeof(FamilyInstance).Name);
            //                break;
            //            default:
            //                //Debug.Print(e.Name + "" + typeof(Element).Name);
            //                break;
            //        }
            //    }
            //    catch
            //    {
            //        continue;
            //    }
            //}


            //Debug.Print(s.ToString());
            return Result.Succeeded;
        }
    }

}
