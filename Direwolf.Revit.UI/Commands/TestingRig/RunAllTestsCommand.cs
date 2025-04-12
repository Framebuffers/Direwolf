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
            async.CreateWolf(new GetDocumentTitle(),
                new List<IConnector> { new DisplayOnScreenConnector(), new Test2Connector() });
            async.CreateWolf(new GetDocumentTitle(), new List<IConnector> { new Test2Connector() });
            async.CreateWolf(new GetDocumentTitle(),
                new List<IConnector> { new DisplayOnScreenConnector(), new DisplayOnScreenConnector() });
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