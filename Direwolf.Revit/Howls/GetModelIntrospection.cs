using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using System.Text;
using System.Threading.Tasks;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls
{
    public record class GetModelIntrospection : RevitHowl
    {
        public GetModelIntrospection(Document doc) => SetRevitDocument(doc);

        public override bool Execute()
        {
            try
            {
                return true;
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Exception thrown. Message:\n{e.Message}"));
                return false;
            }
        }
    }
}
