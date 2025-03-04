using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Direwolf.ManifestGenerator;
using Revit.Async;
using Direwolf.Contracts;
using Direwolf.Definitions;
using System.Windows.Input;
using Autodesk.Revit.DB.Mechanical;
using System.Threading.Tasks;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Direwolf
{
    [Transaction(TransactionMode.Manual)]
    [ManifestAttributes(
        """C:\ProgramData\Autodesk\Revit\Addins\2025""",
        "Framebuffer",
        "github.com\\/Framebuffers",
        "Direwolf",
        "Data Reaper for Revit",
        "Architecture",
        "unknown")]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            RevitTask.Initialize(commandData.Application);
            Document doc = commandData.Application.ActiveUIDocument.Document;
            Direwolf dw = new();
            dw.ExecuteQueryAsync(new RevitDocumentDispatch(doc), "DocumentInformation");

            return Result.Succeeded;
        }

        //    public class ResultData
        //    {
        //        public ResultData(ExternalCommandData e)
        //        {
        //            Command = e;
        //        }
        //        private ExternalCommandData Command { get; set; }

        //        public async void Execute()
        //        {
        //            RevitTask.Initialize(Command.Application);

        //            var t = await RevitTask.RunAsync(
        //              () =>
        //              {
        //                    Document d = Command.Application.ActiveUIDocument.Document;
        //                    Direwolf dw = new();
        //                    dw.ExecuteQueryAsync(new RevitDocumentDispatch(d), "DocumentInformation");
        //                    dw.ShowResultToGUI();
        //                    dw.WriteToFile();

        //                  TaskDialog t = new("TestResult");
        //                  t.MainContent = "Executed";
        //                  t.Show();
        //                  return 0;
        //              }
        //      );
        //        }
        //    }
        //}

        //public class Reap : ICommand
        //{
        //    public bool CanExecute(object parameter) => true;
        //    public event EventHandler CanExecuteChanged;
        //    public async void Execute(object parameter)
        //    {
        //        var doc = parameter as Document;
        //        Direwolf dw = new();
        //        dw.AsyncFetch(new RevitDocumentDispatch(doc)); 
        //        TaskDialog t = new("Result");
        //        t.MainContent = dw.GetData();
        //        t.Show();

        //    }
        //}

        public record class RevitDocumentDispatch : Howler
        {
            public RevitDocumentDispatch(Document revitDoc)
            {
                GetEditableFamilies h = new(revitDoc);
                CreateWolf(new Wolf(), h);
                //Dispatch();
            }
        }

        public record class GetEditableFamilies(Document RevitDocument) : Howl
        {
            public override bool Execute()
            {
                try
                {
                    ICollection<Element> allValidElements = new FilteredElementCollector(RevitDocument)
                        .WhereElementIsNotElementType()
                        .WhereElementIsViewIndependent()
                        .ToElements();

                    var elementsSortedByFamily = new Dictionary<string, List<long>>();

                    Catch c = new();
                    foreach ((Element e, string familyName) in from Element e in allValidElements // create two variables, Element e and string familyType
                                                               let f = e as FamilyInstance // cast each element as a FamilyInstance
                                                               where f is not null // check if it's not null
                                                               let familyName = f.Symbol.Family.Name // assign the family name to the variable familyName
                                                               select (e, familyName)) // get the variables back
                    {
                        if (!elementsSortedByFamily.TryGetValue(familyName, out List<long>? value))
                        {
                            value = [];
                            elementsSortedByFamily[familyName] = value;
                        }
                        value.Add(e.Id.Value);
                    }

                    SendCatchToCallback(new Catch(new Dictionary<string, object>()
                    {
                        ["ElementsByFamily"] = elementsSortedByFamily
                    }));
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
