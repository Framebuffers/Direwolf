using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Direwolf.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Howls
{
    public record class GetUnconnectedPipes : RevitHowl
    {
        public GetUnconnectedPipes(Document doc) => SetRevitDocument(doc);
        public override bool Execute()
        {
            using FilteredElementCollector pipeCollector = new FilteredElementCollector(GetRevitDocument())
                            .OfCategory(BuiltInCategory.OST_PipeCurves)
                            .WhereElementIsNotElementType();

            List<string> unconnectedPipes = [];

            foreach (Element pipeElement in pipeCollector)
            {
                if (pipeElement is Pipe pipe)
                {
                    bool isUnconnected = false;

                    ConnectorSet connectors = pipe.ConnectorManager.Connectors;
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
                        unconnectedPipes.Add($"Pipe Name: {pipe.Name}, Pipe ID: {pipe.Id}");
                    }
                }
            }

            var d = new Dictionary<string, object>()
            {
                ["unconnectedPipes"] = unconnectedPipes
            };
            SendCatchToCallback(new Prey(d));
            return true;


        }
    }
}
