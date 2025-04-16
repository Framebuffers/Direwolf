using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Direwolf.Primitives.Connectors;
using Direwolf.Revit.Howls;
using IConnector = Direwolf.Contracts.IConnector;

namespace Direwolf.Revit.UI.Commands.TestingRig;

[Transaction(TransactionMode.Manual)]
public class RunAllTestsCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        try
        {
            var async = RevitUIDirewolf.CreateInstance(commandData.Application);
            async.CreateWolf(new ElementDefinitions(),
                [
                    new JsonFileConnector(FileName: "Query.json", Path: @"C:\Users\Framebuffer\Desktop")
                ]);
            async.Howl();
            return Result.Succeeded;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result.Failed;
        }
    }
}