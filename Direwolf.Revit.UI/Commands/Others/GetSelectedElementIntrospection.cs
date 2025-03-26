using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Direwolf.Definitions;
using Direwolf.Extensions;
using Direwolf.Revit.Howlers;
using Direwolf.Revit.Introspection;
using Revit.Async;
using System.Diagnostics;

namespace Direwolf.Revit.UI.Commands.Other
{
    [Transaction(TransactionMode.Manual)]
    public class GetSelectedElementIntrospection : IExternalCommand
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
                rh.CreateWolf(new Wolf(), new ElementIntrospection(doc, commandData.Application));
                $"elementsSelected = {elements.Size}".ToConsole();
                Direwolf dw = new(commandData.Application);
                dw.QueueHowler(rh);
                dw.HuntAsync("Extension Test");
                dw.SendAllToDB();
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
