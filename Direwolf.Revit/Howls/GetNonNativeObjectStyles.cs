using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Howls
{
    public record class GetNonNativeObjectStyles : RevitHowl
    {
        public GetNonNativeObjectStyles(Document doc) => SetRevitDocument(doc);

        public override bool Execute()
        {
        }
    }
}
