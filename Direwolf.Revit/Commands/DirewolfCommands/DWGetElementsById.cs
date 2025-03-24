using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.Async;
using System.Diagnostics;

namespace Direwolf.Revit.Commands.DirewolfCommands
{
    [Transaction(TransactionMode.Manual)]
    public class DWGetElementsById: IExternalCommand
    {
        public double TimeTaken { get; private set; } = 0;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Stopwatch benchmarkTimer = new();
            benchmarkTimer.Start();
            RevitTask.Initialize(commandData.Application);
            try
            {
                //var doc = commandData.GetDocument();
                //Direwolf dw = new(commandData.Application);
                //RevitHowler rh = new();
                //dw.HuntAsync(rh, $"{GetType().Name}");
                //benchmarkTimer.Stop();
                //TimeTaken += benchmarkTimer.Elapsed.TotalSeconds;
            }
            catch
            {
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
}
