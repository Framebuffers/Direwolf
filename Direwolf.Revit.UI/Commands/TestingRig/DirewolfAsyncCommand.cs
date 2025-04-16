// using Autodesk.Revit.Attributes;
// using Autodesk.Revit.DB;
// using Autodesk.Revit.UI;
// using Direwolf.Primitives.Connectors;
// using Direwolf.Revit.Howls;
// using Revit.Async;
//
// namespace Direwolf.Revit.UI.Commands.TestingRig;
//
// [Transaction(TransactionMode.Manual)]
// public class DirewolfAsyncCommand : DirewolfRevitCommand
// {
//     public override Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//     {
//         try
//         {
//             RevitTask.Initialize(commandData.Application);
//             var dw = RevitDirewolf.CreateInstance(commandData.Application);
//             dw.CreateWolf(new DocumentTitle(),
//                 new DisplayOnScreenConnector());
//             dw.Howl();
//             return Result.Succeeded;
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine(e);
//             return Result.Failed;
//         }
//     }
// }

