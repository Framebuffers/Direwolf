using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Direwolf.Definitions;
using Direwolf.Revit.Howlers;
using Direwolf.Revit.Howls;
using Direwolf.Revit.Utilities;
using Revit.Async;
using System.Diagnostics;
using static Direwolf.Revit.Utilities.DirewolfExtensions;

namespace Direwolf.Revit.Commands.DirewolfCommands
{
    [Transaction(TransactionMode.Manual)]
    public class DWGetElementInfo : IExternalCommand
    {
        public double TimeTaken { get; private set; } = 0;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Stopwatch benchmarkTimer = new();
            benchmarkTimer.Start();
            RevitTask.Initialize(commandData.Application);
            try
            {
                var doc = commandData.GetDocument();
                Direwolf dw = new(commandData.Application);
                RevitHowler rh = new();
                rh.CreateWolf(new Wolf(), new GetElementInformation(doc));
                dw.HuntAsync(rh, doc.Title);
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
