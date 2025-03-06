using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Definitions.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Examples.Howls
{
    public record class DocumentTest(Document doc) : RevitHowl
    {
        public override bool Execute()
        {
            var rvtdoc = doc;
            var data = new Dictionary<string, object>()
            {
                ["Title"] = rvtdoc.Title
            };
            SendCatchToCallback(new Catch(data));
            return true;
        }
    }

    public record class DynamicDocumentTest(Document doc) : DynamicRevitHowl
    {
        public override bool Execute()
        {
            var rvtdoc = doc;
            dynamic d = new DynamicCatch();
            d.Title = rvtdoc.Title;
            SendCatchToCallback(d);
            return true;

        }
    }

}
