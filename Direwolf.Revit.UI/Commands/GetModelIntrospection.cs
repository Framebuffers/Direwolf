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

    public partial class GetModelIntrospection : IExternalCommand
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
                rh.CreateWolf(new Wolf(), new GetDocumentIntrospection(doc), WolfpackTarget.DB);
                //rh.CreateWolf(new Wolf(), new GetProjectInformationIntrospection(doc), WolfpackTarget.DB);
                //rh.CreateWolf(new Wolf(), new GetProjectSiteIntrospection(doc), WolfpackTarget.DB);
                //rh.CreateWolf(new Wolf(), new GetProjectUnitsIntrospection(doc), WolfpackTarget.DB);
                Direwolf dw = new(commandData.Application);
                dw.QueueHowler(rh);
                dw.HuntAsync("IntrospectionTest");
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
