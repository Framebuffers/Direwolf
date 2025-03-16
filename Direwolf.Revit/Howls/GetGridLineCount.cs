using Autodesk.Revit.DB;
using Direwolf.Definitions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Howls
{
    public record class GetGridLineCount : RevitHowl
    {
        public GetGridLineCount(Document doc) => SetRevitDocument(doc);
        public override bool Execute()
        {
            
            var d = new Dictionary<string, object>()
            {
                ["gridLines"] = new FilteredElementCollector(GetRevitDocument())
                .OfClass(typeof(Grid))
                .GetElementCount()

            };
            SendCatchToCallback(new Prey(d));
            return true;


        }
    }
}
