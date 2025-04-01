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

    public partial class GetDocumentInfo : DirewolfRevitCommand
    {
        public override Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Stopwatch s = new();
            s.Start();
            Document doc = commandData.Application.ActiveUIDocument.Document;
            try
            {
                RevitTask.Initialize(commandData.Application);
                RevitHowler rh = new();
                rh.CreateWolf(new Wolf(), new GetDocumentIntrospection(doc));
                Direwolf dw = new(commandData.Application);
                dw.QueueHowler(rh);
                dw.HuntAsync("DocumentInfo");
                dw.SendAllToDB();

                TaskDialog t = new("Query Information")
                {
                    MainInstruction = "Query executed successfully!",
                    MainContent = "The query has been executed. It is being processed by your database, or it has been saved as a file in your Desktop.",
                    CommonButtons = TaskDialogCommonButtons.Close,
                    DefaultButton = TaskDialogResult.Close
                };
                t.Show();
            }
            catch (Exception e)
            {
                TaskDialog t = new("Query Information")
                {
                    MainInstruction = "Query has failed.",
                    MainContent = $"The query has raised the following Exception: {e.Message}",
                    CommonButtons = TaskDialogCommonButtons.Close,
                    DefaultButton = TaskDialogResult.Close
                };
                t.Show();
                return Result.Failed;
            }

            s.Stop();
            Debug.Print($"Time taken: {s.Elapsed.Seconds}");
            return Result.Succeeded;
        }
    }
}
