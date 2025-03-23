using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Direwolf.Definitions;
using Direwolf.Revit.Howlers;
using Direwolf.Revit.Howls;
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
                RevitHowler rh = new();
                rh.CreateWolf(new Wolf(), GetDocumentIntrospection(commandData.Application.ActiveUIDocument.Document));
                return Result.Succeeded;
            }
            catch
            {
                return Result.Failed;
            }
        }
    }
}
