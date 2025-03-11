using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System.Text.Json;
using System.Diagnostics;
using Direwolf.Definitions.Dynamics;
using Direwolf.Definitions;
using Direwolf.Revit.Howlers.Dynamics;
using Direwolf.Revit.Howlers;
using Direwolf.Revit.Utilities;
using Direwolf.Revit.Howls.Dynamics;
using Direwolf.Revit.Howls;
using Revit.Async;

namespace Direwolf.Revit
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
                RevitTask.Initialize(commandData.Application);

                //DynamicRevitHowler drh = new();
                //drh.CreateWolf(new DynamicWolf(), new DynamicDocumentTest(doc));
                //drh.CreateWolf(new DynamicWolf(), new DynamicElementInformation(doc));
                //drh.CreateWolf(new DynamicWolf(), new DynamicIdByFamily(doc));

                //RevitHowler rh = new();
                //rh.CreateWolf(new Wolf(), new GetElementIdByFamily(doc));
                //rh.CreateWolf(new Wolf(), new DocumentTest(doc));
                //rh.CreateWolf(new Wolf(), new GetElementInformation(doc));

                Direwolf dw2 = new(commandData.Application);
                //foreach (var v in dw2.Database)
                //{
                //    foreach (var k in v.Result)
                //    {
                //        Debug.Print(k.Key);
                //    }
                //}

                //dw2.QueueHowler(drh);
                //dw2.QueueHowler(rh);
                //dw2.HuntAsync("Async Dynamic and Static Queries");
                s.Stop();

                //Helpers.GenerateNewWindow("Request", $"{dw2.Database.Length}");

                //Debug.Print(JsonSerializer.Serialize(dw2.Queries));
            }
            catch
            {
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}
