using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System.Text.Json;
using System.Diagnostics;
using Direwolf.Definitions;
using Direwolf.Revit.Howlers;
using Revit.Async;
using Direwolf.Revit.Benchmarking;

namespace Direwolf.Revit.UI.Commands
{
    /// <summary>
    /// Benchmark code.
    /// </summary>
    [Transaction(TransactionMode.Manual)]

    public partial class GetModelHealth : IExternalCommand
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
                rh.CreateWolf(new Wolf(), new ModelHealthReaper(doc));
                Direwolf dw = new(commandData.Application);
                dw.QueueHowler(rh);
                dw.HuntAsync("Model Health");
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
