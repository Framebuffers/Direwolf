using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Direwolf.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Howls
{
    public record class GetUnconnectedDucts : RevitHowl
    {
        public GetUnconnectedDucts(Document doc) => SetRevitDocument(doc);
        public override bool Execute()
        {
            using FilteredElementCollector ductCollector = new FilteredElementCollector(GetRevitDocument())
                           .OfCategory(BuiltInCategory.OST_DuctCurves)
                           .WhereElementIsNotElementType();

            List<string> unconnectedDucts = [];

            foreach (Element ductElement in ductCollector)
            {
                if (ductElement is Duct duct)
                {
                    bool isUnconnected = false;

                    ConnectorSet connectors = duct.ConnectorManager.Connectors;
                    foreach (Connector connector in connectors)
                    {
                        if (!connector.IsConnected)
                        {
                            isUnconnected = true;
                            break;
                        }
                    }

                    if (isUnconnected)
                    {
                        unconnectedDucts.Add($"Duct Name: {duct.Name}, Duct ID: {duct.Id}");
                    }
                }
            }

            var d = new Dictionary<string, object>()
            {
                ["unconnectedDucts"] = unconnectedDucts
            };
            SendCatchToCallback(new Prey(d));
            return true;


        }
    }
}
