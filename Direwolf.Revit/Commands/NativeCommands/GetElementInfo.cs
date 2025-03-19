using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Diagnostics;
using static Direwolf.Revit.Utilities.DirewolfExtensions;

namespace Direwolf.Revit.Commands.NativeCommands;

[Transaction(TransactionMode.Manual)]
public class GetElementInfo : IExternalCommand
{
    public double TimeTaken { get; private set; } = 0;

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        
        Stopwatch benchmarkTimer = new();
        benchmarkTimer.Start();
        try
        {
            var doc = GetRevitApplicationInstances.GetDocument(commandData);
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

    private static Dictionary<string, object> RunBenchmark(Document RevitDocument)
    {
        try
        {
            Dictionary<string, List<Dictionary<string, object>>> Catches = [];
            ICollection<Element> allValidElements = new FilteredElementCollector(RevitDocument)
                .WhereElementIsNotElementType()
                .WhereElementIsViewIndependent()
                .ToElements();
            Dictionary<string, List<Element>> elementsSortedByFamily = [];

            foreach ((Element e, string familyName) in from Element e in allValidElements
                                                       let f = e as FamilyInstance
                                                       where f is not null
                                                       let familyName = f.Symbol.Family.Name
                                                       select (e, familyName))
            {
                if (!elementsSortedByFamily.TryGetValue(familyName, out List<Element>? value))
                {
                    value = new List<Element>();
                    elementsSortedByFamily[familyName] = value;
                }
                value.Add(e);
            }

            foreach (KeyValuePair<string, List<Element>> family in elementsSortedByFamily)
            {
                List<Dictionary<string, object>> elementData = [];
                elementData.AddRange(family.Value.Select(Common.ExtractElementData));

                if (Catches.TryGetValue(family.Key, out List<Dictionary<string, object>>? existingElementData))
                {
                    existingElementData.AddRange(elementData);
                }
                else
                {
                    Catches[family.Key] = elementData;
                }
            }

            return new Dictionary<string, object>(new Dictionary<string, object>()
            {
                ["ElementData"] = Catches
            });
        }
        catch (Exception e)
        {
            TaskDialog t = new("error")
            {
                MainContent = e.Message + "\n" + e.StackTrace
            };
            return [];
        }
    }
}

