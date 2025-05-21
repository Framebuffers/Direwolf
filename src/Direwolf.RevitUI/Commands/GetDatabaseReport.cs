using System.Diagnostics;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Direwolf.EventArgs;
using Direwolf.Extensions;
using Direwolf.Sources.InternalDB;

namespace Direwolf.RevitUI.Commands;

[Transaction(TransactionMode.Manual)]
public class GetDatabaseReport : IExternalCommand
{
    private readonly Database? _db = Direwolf.GetDatabase();

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        Debug.Print(_db?.Debug_GetDatabaseReport());

        return Result.Succeeded;
    }
}