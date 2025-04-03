using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Direwolf.Definitions;
using Direwolf.Revit.Howlers;
using Direwolf.Revit.Howls;
using Revit.Async;

namespace Direwolf.Revit.UI.Commands;

/// <summary>
///     Takes a model, iterates through the entire model in search of a given set of indicators, and returns the amount of
///     elements that fall into each category.
/// </summary>
[Transaction(TransactionMode.Manual)]
public class GetModelInfo : DirewolfRevitCommand
{
    public override Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        Stopwatch s = new();
        s.Start();
        var doc = commandData.Application.ActiveUIDocument.Document;
        try
        {
            RevitTask.Initialize(commandData.Application);
            RevitHowler rh = new();
            rh.CreateWolf(new Wolf(), new GetModelSnapshot(doc));
            Direwolf dw = new(commandData.Application);
            dw.QueueHowler(rh);
            dw.HuntAsync("ModelInfo");
            dw.SendAllToDB();

            TaskDialog t = new("Query Information")
            {
                MainInstruction = "Query executed successfully!",
                MainContent =
                    "The query has been executed. It is being processed by your database, or it has been saved as a file in your Desktop.",
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