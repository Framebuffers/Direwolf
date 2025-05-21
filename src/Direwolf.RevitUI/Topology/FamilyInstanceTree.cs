using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Direwolf.Extensions;
using Direwolf.Sources.InternalDB;

using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace Direwolf.RevitUI.Topology;

[Transaction(TransactionMode.Manual)]
public class FamilyInstanceTree : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        try
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            Database db = new(doc);
            db.PopulateDatabase();
            TaskDialog.Show("Elements in DB",
                            db.GetDatabaseCount().ToString());

            return Result.Succeeded;
        }
        catch (Exception e) { return Result.Failed; }
    }


    private static string GetFamilyName(Element element)
    {
        return element is ElementType elementType
            ? elementType.FamilyName
            : string.Empty;
    }
}