using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Howls
{
    public record class GetElementIntrospection : RevitHowl
    {
        public GetElementIntrospection(Document doc) => SetRevitDocument(doc);
    }
}
