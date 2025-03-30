using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Direwolf.Revit.UI.Commands.Other
{
    [Transaction(TransactionMode.Manual)]
    public class GetElementsPerView : IExternalCommand
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
