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
    public record class GetFamilyInstanceIntrospection : RevitHowl
    {
        public GetFamilyInstanceIntrospection(Document doc, FamilyInstance? f)
        {
            ArgumentNullException.ThrowIfNull(doc);
            ArgumentNullException.ThrowIfNull(f);
            SetRevitDocument(doc);
            _f = f;
        }

        private FamilyInstance? _f;
        public override bool Execute()
        {
            try
            {
                if(_f is not null)
                SendCatchToCallback(new Prey(new FamilyInstanceIntrospection(_f)));
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
