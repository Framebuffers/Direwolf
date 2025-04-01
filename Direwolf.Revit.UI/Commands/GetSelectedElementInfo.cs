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
                rh.CreateWolf(new Wolf(), new ElementSnapshot(doc, commandData.Application));
                Direwolf dw = new(commandData.Application);
                dw.QueueHowler(rh);
                dw.HuntAsync("ElementInfo");
                var s = StopTime();
                Debug.Print($"Time taken: {s}");
                dw.SendAllToDB();
                TaskDialog t = new("Query Information")
                {
                    MainInstruction = "Query executed successfully!",
                    MainContent = "The query has been executed. It is being processed by your database, or it has been saved as a file in your Desktop.",
                    DefaultButton = TaskDialogResult.Ok
                };
                t.Show();

            }
            catch(Exception e)
            {
                TaskDialog t = new("Query Information")
                {
                    MainInstruction = "Query has failed.",
                    MainContent = $"The query has raised the following Exception: {e.Message}",
                    DefaultButton = TaskDialogResult.Ok
                };
                t.Show();

                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}