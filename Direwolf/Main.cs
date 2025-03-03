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

            //Direwolf dw = new();
            //Reap r = new();
            //r.Execute(commandData.Application.ActiveUIDocument.Document);
            //var r = dw.AsyncFetch(new DocumentHowler(commandData.Application.ActiveUIDocument.Document));
            Document d = commandData.Application.ActiveUIDocument.Document;
            Direwolf dw = new();
            dw.ExecuteRevitQueryAsync(commandData, new DocumentHowler(d), new DocumentInfoHowl(d), "DocumentInformation");
            dw.ShowResultToGUI();
            //UIApplication uiapp = commandData.Application;
            //Document doc = uiapp.ActiveUIDocument.Document;

            ////Define a reference Object to accept the pick result  
            //Reference pickedref;

            ////Pick a group  
            //Selection sel = uiapp.ActiveUIDocument.Selection;
            //pickedref = sel.PickObject(ObjectType.Element, "Please select a group");

            //Element elem = doc.GetElement(pickedref);
            //Group? group = elem as Group;

            ////Pick point  
            //XYZ point = sel.PickPoint("Please pick a point to place group");

            ////Place the group  
            //Transaction trans = new Transaction(doc);
            //trans.Start("Lab");
            //doc.Create.PlaceGroup(point, group?.GroupType);
            //trans.Commit();

            return Result.Succeeded;
        }

        public class ResultData
        {
            public ResultData(ExternalCommandData e)
            {
                Command = e;
            }
            private ExternalCommandData Command { get; set; }
            
            public async void Execute()
            {
                RevitTask.Initialize(Command.Application);

                var t = await RevitTask.RunAsync(
                  () =>
                  {

                      //var howler = new DocumentHowler(commandData.Application.ActiveUIDocument.Document);
                      //howler.CreateWolf(new GenericWolf(), new DocumentInfoHowl(commandData.Application.ActiveUIDocument.Document));
                      //howler.Dispatch();
                      //var r = new Dictionary<string, object>()
                      //{
                      //    ["result"] = howler.ToString() ?? string.Empty
                      //};
                      //Catch c = new(r);

                      TaskDialog t = new("TestResult");
                      t.MainContent = JsonSerializer.Serialize(c);
                      t.Show();
                      return 0;
                  }
          );
            }
        }
    }
    
    //public class Reap : ICommand
    //{
    //    public bool CanExecute(object parameter) => true;
    //    public event EventHandler CanExecuteChanged;
    //    public async void Execute(object parameter)
    //    {
    //        var doc = parameter as Document;
    //        Direwolf dw = new();
    //        dw.AsyncFetch(new DocumentHowler(doc)); 
    //        TaskDialog t = new("Result");
    //        t.MainContent = dw.GetData();
    //        t.Show();

    //    }
    //}

    public record DocumentHowler : Howler
    {
        public DocumentHowler(Document revitDoc)
        {
            DocumentInfoHowl h = new(revitDoc);
            CreateWolf(new Wolf(), h);
            //Dispatch();
        }
    }

    public record DocumentInfoHowl(Document RevitDocument) : Howl
    {
        public override bool Execute()
        {
            try
            {
                PushCatchesToWolf(new Catch(new Dictionary<string, object>()
                {
                    ["DocumentPath"] = RevitDocument.PathName,
                    ["DocumentTitle"] = RevitDocument.Title
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