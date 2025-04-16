using System.Diagnostics;
using Autodesk.Revit.DB;
using Direwolf.Contracts;
using Direwolf.Revit.Contracts;
using Direwolf.Revit.Definitions;
using Direwolf.Revit.Definitions.Primitives;
using Direwolf.Revit.Extensions;

namespace Direwolf.Revit.Howls;

public record ElementDefinitions : IRevitHowl
{
    public string? Name { get; set; } = "ElementDefinitions";
    public IWolf? Wolf { get; set; }

    public RevitWolfpack? ExecuteHunt()
    {
        Stopwatch s = new();
        s.Start();
        if (Document is null) throw new NullReferenceException("Document is null");
        var episode = new RevitDocumentEpisode(Document);
        Dictionary<string, List<ElementEntity?>> results = [];
        foreach (var element in new FilteredElementCollector(Document).WhereElementIsNotElementType()
                     .WhereElementIsElementType().ToElements())
        {
            if (element?.Category is null) continue;
            var validElement = Document.GetElement(element.UniqueId);
            if (validElement is null) continue;
            var builtInCategory = validElement.Category.BuiltInCategory.ToString();
            validElement.TryGetElementEntity(episode, out var elementEntity);
            if (elementEntity is null) continue;
            if (!results.TryGetValue(builtInCategory, out var elements))
            {
                elements = (List<ElementEntity?>) [];
                results.Add(element.Category.BuiltInCategory.ToString(), elements);
            }

            results[builtInCategory].Add(elementEntity);
        }

        s.Stop();
        return RevitWolfpack.New(Name, episode, results, s.ElapsedMilliseconds, true);
    }

    public Document? Document { get; set; }

    IWolfpack? IHowl.ExecuteHunt()
    {
        return ExecuteHunt();
    }
}