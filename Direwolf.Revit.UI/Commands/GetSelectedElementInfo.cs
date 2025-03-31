using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System.Diagnostics;
using Direwolf.Revit.Howlers;
using Revit.Async;
using Direwolf.Definitions;
using Direwolf.Revit.Introspection;
using Direwolf.Revit.Definitions;

namespace Direwolf.Revit.UI.Commands
{

    /// <summary>
    /// </summary>
    [Transaction(TransactionMode.Manual)]

    public partial class GetSelectedElementInfo : DirewolfRevitCommand
    {
        public override Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            StartTime();
            Document doc = commandData.Application.ActiveUIDocument.Document;
            try
            {
                RevitTask.Initialize(commandData.Application);
                RevitHowler rh = new();
                rh.CreateWolf(new Wolf(), new ElementIntrospection(doc, commandData.Application));
                Direwolf dw = new(commandData.Application);
                dw.QueueHowler(rh);
                dw.HuntAsync("ElementInformation");
                var s = StopTime();
                Debug.Print($"Time taken: {s}");
                dw.SendAllToDB();
            }
            catch
            {
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}