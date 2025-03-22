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
    public record class GetParameterIntrospection : RevitHowl
    {
        public GetParameterIntrospection(Document doc, Parameter? p)
        {
            ArgumentNullException.ThrowIfNull(doc);
            ArgumentNullException.ThrowIfNull(p);
            SetRevitDocument(doc);
            _p = p;
        }
        private Parameter? _p;
        public override bool Execute()
        {
            try
            {
                if (_p is not null)
                    SendCatchToCallback(new Prey(new ParameterIntrospection(_p)));
                return true;
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Exception thrown: {e.Message}"));
                return false;
            }
        }
    }
}
