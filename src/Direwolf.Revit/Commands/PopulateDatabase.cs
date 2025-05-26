using System.Diagnostics;

using Autodesk.Revit.Attributes;

using Direwolf.Extensions;

using Nice3point.Revit.Toolkit.External;

namespace Direwolf.Revit.Commands;

/// <summary>
///     External command entry point
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class PopulateDatabase : ExternalCommand
{
    public override void Execute()
    {
        Revit.Application.Direwolf?.PopulateDatabase();
        Debug.Print(Revit.Application.Direwolf?.GetDatabaseCount().ToString());
    }
}