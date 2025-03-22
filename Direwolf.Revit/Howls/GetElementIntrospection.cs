using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Revit.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Howls
{
    public record class GetElementIntrospection : RevitHowl
    {
        public GetElementIntrospection(Document? doc, Element? element)
        {
            ArgumentNullException.ThrowIfNull(doc);
            ArgumentNullException.ThrowIfNull(element);

            SetRevitDocument(doc);
            _e = element;
        }

        private readonly Element _e;
        public override bool Execute()
        {
            try
            {
                SendCatchToCallback(new Prey(new ElementIntrospection(_e)));
            }
            catch
            {
                SendCatchToCallback(new Prey(new Dictionary<string, object>()
                {
                    ["elementIntrospection"] = "Could not get information."
                }));
            }
            return true;
        }
    }
}
