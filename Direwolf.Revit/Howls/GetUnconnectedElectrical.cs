using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls
{
    public record class GetUnconnectedElectrical : RevitHowl
    {
        public GetUnconnectedElectrical(Document doc) => SetRevitDocument(doc);
        public override bool Execute()
        {
            using FilteredElementCollector electricalCollector = new FilteredElementCollector(GetRevitDocument())
                            .OfCategory(BuiltInCategory.OST_ElectricalFixtures)
                            .WhereElementIsNotElementType();

            List<string> unconnectedConnections = [];

            foreach (Element electricalElement in electricalCollector)
            {
                using MEPModel mepModel = ((FamilyInstance)electricalElement).MEPModel;
                if (mepModel != null)
                {
                    using ConnectorSet connectors = mepModel.ConnectorManager.Connectors;
                    foreach (Connector connector in connectors)
                    {
                        if (!connector.IsConnected)
                        {
                            unconnectedConnections.Add($"Element Name: {electricalElement.Name}, ID: {electricalElement.Id}, Connector ID: {connector.Id}");
                        }
                    }
                }

            }
            var d = new Dictionary<string, object>()
            {
                ["unconnectedElectrical"] = unconnectedConnections
            };
            SendCatchToCallback(new Prey(d));
            return true;

        }
    }
}
