// using Autodesk.Revit.Attributes;
// using Autodesk.Revit.DB;
// using Autodesk.Revit.UI;
// using Direwolf.Primitives.Connectors;
// using Direwolf.Revit.Howls;
// using Revit.Async;
//
// namespace Direwolf.Revit.UI.Commands;
//
// /// <summary>
// ///     Gets all the parameters and instance data from the selected elements.
// /// </summary>
// [Transaction(TransactionMode.Manual)]
// public class GetSelectedElementInfo : DirewolfRevitCommand
// {
//     public override Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//     {
//         StartTime();
//         var doc = commandData.Application.ActiveUIDocument.Document;
//         try
//         {
//             if (commandData.Application.ActiveUIDocument.Selection.GetElementIds().Count == 0)
//             {
//                 TaskDialog f = new("Query Information")
//                 {
//                     MainInstruction = "No elements have been selected",
//                     MainContent = "Select an element and try again.",
//                     CommonButtons = TaskDialogCommonButtons.Close,
//                     DefaultButton = TaskDialogResult.Close
//                 };
//                 f.Show();
//                 return Result.Failed;
//             }
//
//             RevitTask.Initialize(commandData.Application);
//             var dw = RevitDirewolf.CreateInstance(commandData.Application);
//             dw.CreateWolf(
//                 new GetModelSnapshot(), 
//                 new DisplayOnScreenConnector());
//             dw.Howl(); 
//             
//           
//             TaskDialog t = new("Query Information")
//             {
//                 MainInstruction = "Query executed successfully!",
//                 MainContent =
//                     "The query has been executed. It is being processed by your database, or it has been saved as a file in your Desktop.",
//                 CommonButtons = TaskDialogCommonButtons.Close,
//                 DefaultButton = TaskDialogResult.Close
//             };
//             t.Show();
//         }
//         catch (Exception e)
//         {
//             TaskDialog t = new("Query Information")
//             {
//                 MainInstruction = "Query has failed.",
//                 MainContent = $"The query has raised the following Exception: {e.Message}",
//                 CommonButtons = TaskDialogCommonButtons.Close,
//                 DefaultButton = TaskDialogResult.Close
//             };
//             t.Show();
//
//             return Result.Failed;
//         }
//
//         return Result.Succeeded;
//     }
// }

