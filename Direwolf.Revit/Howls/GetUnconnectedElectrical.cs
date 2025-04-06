using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls;

public record GetUnconnectedElectrical : RevitHowl
{
    public GetUnconnectedElectrical(Document doc)
    {
        SetRevitDocument(doc);
    }

    public override bool Hunt()
    {
        using var electricalCollector = new FilteredElementCollector(GetRevitDocument())
            .OfCategory(BuiltInCategory.OST_ElectricalFixtures)
            .WhereElementIsNotElementType();

        List<string> unconnectedConnections = [];

        foreach (var electricalElement in electricalCollector)
        {
            using var mepModel = ((FamilyInstance)electricalElement).MEPModel;
            if (mepModel != null)
            {
                using var connectors = mepModel.ConnectorManager.Connectors;
                foreach (Connector connector in connectors)
                    if (!connector.IsConnected)
                        unconnectedConnections.Add(
                            $"Element Name: {electricalElement.Name}, ID: {electricalElement.Id}, Connector ID: {connector.Id}");
            }
        }

        var d = new Dictionary<string, object>
        {
            ["unconnectedElectrical"] = unconnectedConnections
        };
        SendCatchToCallback(new Prey(d));
        return true;
    }
}