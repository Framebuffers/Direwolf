using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace Direwolf.Revit.Howls
{
    public record class GetFamilyIntrospection : RevitHowl
    {
        public GetFamilyIntrospection(Document doc) => SetRevitDocument(doc);
    }
}
