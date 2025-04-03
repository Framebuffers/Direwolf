using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls;

public record class GetUnconnectedPipes : RevitHowl
{
    public GetUnconnectedPipes(Document doc)
    {
        SetRevitDocument(doc);
    }

    public override bool Execute()
    {
        using var pipeCollector = new FilteredElementCollector(GetRevitDocument())
            .OfCategory(BuiltInCategory.OST_PipeCurves)
            .WhereElementIsNotElementType();

        List<string> unconnectedPipes = [];

        foreach (var pipeElement in pipeCollector)
            if (pipeElement is Pipe pipe)
            {
                var isUnconnected = false;

                var connectors = pipe.ConnectorManager.Connectors;
                foreach (Connector connector in connectors)
                    if (!connector.IsConnected)
                    {
                        isUnconnected = true;
                        break;
                    }

                if (isUnconnected) unconnectedPipes.Add($"Pipe Name: {pipe.Name}, Pipe ID: {pipe.Id}");
            }

        var d = new Dictionary<string, object>
        {
            ["unconnectedPipes"] = unconnectedPipes
        };
        SendCatchToCallback(new Prey(d));
        return true;
    }
}