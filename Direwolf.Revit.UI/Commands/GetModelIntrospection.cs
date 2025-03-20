using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.UI.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class GetModelIntrospection : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {

                return Result.Succeeded;
            }
            catch
            {
                return Result.Failed;
            }
        }
    }
}
