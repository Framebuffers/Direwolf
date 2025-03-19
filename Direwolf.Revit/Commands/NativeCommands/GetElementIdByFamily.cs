using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Direwolf.Revit.Utilities;
using System.Diagnostics;
using System.Transactions;
using static Direwolf.Revit.Utilities.DirewolfExtensions;

namespace Direwolf.Revit.Commands.NativeCommands;

[Transaction(TransactionMode.Manual)]
public class GetElementIdByFamily : IExternalCommand
{
    public double TimeTaken { get; private set; } = 0;
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        Stopwatch benchmarkTimer = new();
        benchmarkTimer.Start();
        try
        {
            var doc = commandData.GetDocument();
            Common.WriteToFile($"{GetType().Name}_Native.json", RunBenchmark(doc));
            benchmarkTimer.Stop();
            TimeTaken += benchmarkTimer.Elapsed.TotalSeconds;
        }
        catch
        {
            return Result.Failed;
        }

        return Result.Succeeded;
    }

    public static Dictionary<string, object> RunBenchmark(Document RevitDocument)
    {
        ICollection<Element> allValidElements = Common.GetAllValidElements(RevitDocument);
        
        var elementsSortedByFamilyNative = new Dictionary<string, List<long>>();
        foreach ((Element e, string familyName) in from Element e in allValidElements
                                                   let f = e as FamilyInstance 
                                                   where f is not null
                                                   let familyName = f.Symbol.Family.Name           
                                                   select (e, familyName)) 
        {
            if (!elementsSortedByFamilyNative.TryGetValue(familyName, out List<long>? value))
            {
                value = [];
                elementsSortedByFamilyNative[familyName] = value;
            }
            value.Add(e.Id.Value);
        }
        return new Dictionary<string, object>()
        {
            ["ElementsByFamily"] = elementsSortedByFamilyNative
        };
    }

}

