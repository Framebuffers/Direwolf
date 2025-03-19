using Autodesk.Revit.DB;
using Direwolf.Revit.ElementFilters;
using Direwolf.Revit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Howls
{
    public record class GetExtensionTest : RevitHowl
    {
        public GetExtensionTest(Autodesk.Revit.DB.Document doc) => SetRevitDocument(doc);
        public override bool Execute()
        {
            SendCatchToCallback(GetRevitDocument().GetAnnotativeElements());
            SendCatchToCallback(GetRevitDocument().GetDesignOptions());
            return true;
        }

    }
}
