using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System.Diagnostics;
using Direwolf.Revit.Howlers;
using Revit.Async;
using Direwolf.Definitions;
using Direwolf.Revit.Howls;
using Direwolf.Extensions;
using Direwolf.Revit.Introspection;

namespace Direwolf.Revit.UI.Commands
{

    /// <summary>
    /// Benchmark code.
    /// </summary>
    [Transaction(TransactionMode.Manual)]

    public partial class GetModelHealth : DirewolfRevitCommand
    {
        public static LocationInformation GetButtonData()
        {
            return new LocationInformation()
            {
                ButtonName = "GetModelHealth",
                Descriptor = "Get Model Health",
                AssemblyLocation = typeof(GetModelHealth).Assembly.Location,
                ClassName = typeof(GetModelHealth).FullName ?? string.Empty
            };
        }

        public override Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            StartTime();
            UIApplication app = commandData.Application;
            Document doc = app.ActiveUIDocument.Document;
            IEnumerable<ElementId> selectedElementIds = app.ActiveUIDocument.Selection.GetElementIds();
            BasicFileInfo fileInfo = BasicFileInfo.Extract(doc.PathName);

            try
            {
                RevitTask.Initialize(app);
                //PrintAsmInfo();
                foreach (var r in app.GetRibbonPanels())
                {
                    r.Name.ToConsole();
                    r.Title.ToConsole();
                    r.Visible.ToString().ToConsole();
                }
                RevitHowler rh = new();
                rh.CreateWolf(new Wolf(), new ElementIntrospection(doc, app));
                Direwolf dw = new(app);
                dw.QueueHowler(rh);
                dw.HuntAsync("Extension Test");
                var s = StopTime();
                Debug.Print($"ProjectSpecTime taken to dispatch: {s}");
                dw.SendAllToDB();
            }
            catch
            {
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
}
