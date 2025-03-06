using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Direwolf.Definitions;
using Direwolf.Examples.Howlers;
using Direwolf.Examples.Howls;
using Revit.Async;
using System.Diagnostics;
using static Direwolf.Helpers;

namespace Direwolf.Examples.RevitCommands
{
    [Transaction(TransactionMode.Manual)]
    public class Benchmark_DWGetElementsById: IExternalCommand
    {
        public double TimeTaken { get; private set; } = 0;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Stopwatch benchmarkTimer = new();
            benchmarkTimer.Start();
            RevitTask.Initialize(RevitAppDoc.GetApplication(commandData));
            try
            {
                var doc = RevitAppDoc.GetDocument(commandData);
                Direwolf dw = new();
                RevitHowler rh = new();
                rh.CreateWolf(new Wolf(), new GetElementIdByFamily(doc));
                dw.HuntAsync(rh, $"{GetType().Name}");
                benchmarkTimer.Stop();
                TimeTaken += benchmarkTimer.Elapsed.TotalSeconds;
            }
            catch
            {
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
}
