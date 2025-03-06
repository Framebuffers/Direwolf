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
using Direwolf.Examples.Howlers;
using Direwolf.Definitions;

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
                Revit.Async.RevitTask.Initialize(commandData.Application);
                //Helpers.GenerateNewWindow("test", "test");
                //RevitHowler rh = new();

                //Debug.Print("hi");

                DynamicRevitHowler drh = new();
                drh.CreateWolf(new DynamicWolf(), new DynamicDocumentTest(doc));
                drh.CreateWolf(new DynamicWolf(), new DynamicElementInformation(doc));
                drh.CreateWolf(new DynamicWolf(), new DynamicIdByFamily(doc));

                RevitHowler rh = new();
                rh.CreateWolf(new Wolf(), new GetElementIdByFamily(doc));
                rh.CreateWolf(new Wolf(), new DocumentTest(doc));
                rh.CreateWolf(new Wolf(), new GetElementInformation(doc));

                //Direwolf dw = new(commandData.Application);
                ////dw.QueueHowler(drh);
                //dw.QueueHowler(rh);

                ////Console.WriteLine(dw.GetQueueInfo());

                //dw.Hunt("Dynamic and Static queries");

                //Debug.Print(JsonSerializer.Serialize(dw.Queries));
                ////Console.WriteLine(JsonSerializer.Serialize(dw.DynamicQueries));
                //dw.WriteQueriesToJson();

                Direwolf dw2 = new(commandData.Application);
                dw2.QueueHowler(drh);
                dw2.QueueHowler(rh);
                dw2.HuntAsync("Async Dynamic and Static Queries");
                //s.Stop();

            Helpers.GenerateNewWindow("Completed complete async request", $"Time Taken: {s.Elapsed.TotalSeconds}s");

                Debug.Print(JsonSerializer.Serialize(dw2.Queries));
                //Console.WriteLine(JsonSerializer.Serialize(dw2.DynamicQueries));
                //dw2.WriteQueriesToJson();
            }
            catch
            {
                return Result.Failed;
            }
    
            return Result.Succeeded;
        }
    }
}
