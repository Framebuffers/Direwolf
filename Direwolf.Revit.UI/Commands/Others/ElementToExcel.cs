using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.UI.Commands.Other
{
    [Transaction(TransactionMode.Manual)]
    public class ElementToExcel : IExternalCommand
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
