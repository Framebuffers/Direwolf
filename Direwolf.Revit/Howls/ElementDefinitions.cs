using System.Diagnostics;
using Autodesk.Revit.DB;
using Direwolf.Contracts;
using Direwolf.Revit.Contracts;
using Direwolf.Revit.Definitions;
using Direwolf.Revit.Definitions.Primitives;
using Direwolf.Revit.Extensions;

namespace Direwolf.Revit.Howls;

public record ElementDefinitions : RevitHowl
{
    public override RevitWolfpack? ExecuteHunt()
    {
        Stopwatch s = new();
        s.Start();
        var doc = GetRevitDocument();
        if (doc is null) throw new NullReferenceException("Document is null");
        var episode = new RevitDocumentEpisode(doc);
        Dictionary<string, List<ElementEntity?>> results = [];
        foreach (var element in new FilteredElementCollector(Document).WhereElementIsNotElementType().ToElements())
        {
            if (element?.Category is null) continue;
            // var validElement = doc.GetElement(element.UniqueId);
            element.TryGetElementEntity(episode, out var elementEntity);
            if (elementEntity is null) continue;
            var category = element.Category.BuiltInCategory.ToString();
            if (!results.TryGetValue(category, out var elements))
            {
                elements = (List<ElementEntity?>) [];
                results.Add(category, elements);
            }
            results[category].Add(elementEntity);
        }

        s.Stop();
        return RevitWolfpack.New(Name, episode, results, s.ElapsedMilliseconds, true);
    }
}