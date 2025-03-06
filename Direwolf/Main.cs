using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Revit.Async;
using System.Text.Json;
using System.Diagnostics;
using Direwolf.Examples.RevitCommands;
using Direwolf.Examples.Howls;
using Direwolf.Examples.Howlers.Dynamics;
using Direwolf.Definitions.Dynamics;
using Direwolf.Examples.Howls.Dynamics;

namespace Direwolf
{
    /// <summary>
    /// Benchmark code.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public partial class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Stopwatch s = new();
            s.Start();
            Document doc = commandData.Application.ActiveUIDocument.Document;
            try
            {
                DynamicRevitHowler drw = new();
                drw.CreateWolf(new DynamicWolf(), new DynamicIdByFamily(doc));
                drw.CreateWolf(new DynamicWolf(), new DynamicElementInformation(doc));

                Direwolf dw = new(drw);
                Helpers.GenerateNewWindow("Direwolf Queue", dw.GetQueueInfo());

                dw.HuntAsync("RevitInformation");
                Helpers.GenerateNewWindow("Direwolf Queries", dw.GetQueryInfo());

                dw.WriteDynamicQueriesToJson();
            }
            catch
            {
                return Result.Failed;
            }
            s.Stop();

            Helpers.GenerateNewWindow("Completed complete async request", $"Time Taken: {s.Elapsed.TotalSeconds}s");

            return Result.Succeeded;

        }

    }
}
