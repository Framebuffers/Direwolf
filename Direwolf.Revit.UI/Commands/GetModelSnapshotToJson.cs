using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System.Diagnostics;
using Direwolf.Revit.Howlers;
using Revit.Async;
using Direwolf.Definitions;
using Direwolf.Revit.Howls;

namespace Direwolf.Revit.UI.Commands
{

    /// <summary>
    /// Benchmark code.
    /// </summary>
    [Transaction(TransactionMode.Manual)]

    public partial class GetModelSnapshotToJson : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Stopwatch s = new();
            s.Start();
            Document doc = commandData.Application.ActiveUIDocument.Document;
            try
            {
                RevitTask.Initialize(commandData.Application);
                RevitHowler rh = new();
                rh.CreateWolf(new Wolf(), new GetModelSnapshot(doc));
                Direwolf dw = new(commandData.Application);
                dw.QueueHowler(rh);
                dw.HuntAsync("GetModelSnapshot");
                dw.WriteQueriesToJson();
            }
            catch
            {
                return Result.Failed;
            }

            s.Stop();
            Debug.Print($"Time taken: {s.Elapsed.Seconds}");
            return Result.Succeeded;
        }
    }
}
