using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Examples.Howls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Examples.Howlers
{
    public record class RevitParameterDispatch : Howler
    {
        public RevitParameterDispatch(Document revitDoc)
        {
            GetElementInformation h = new(revitDoc);
            CreateWolf(new Wolf(), h);
        }
    }
}
