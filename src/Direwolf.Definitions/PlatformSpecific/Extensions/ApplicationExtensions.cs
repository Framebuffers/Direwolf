using System.Collections;
using System.Runtime.Caching;
using System.Text;
using System.Text.Json;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Direwolf.Definitions.PlatformSpecific.Extensions;

public static class ApplicationExtensions
{
    public static IEnumerable<RevitElement?> ParseByUniqueIdentifier(this Document doc, string[] elementUniqueId)
    {
        return elementUniqueId.Select(doc.GetElement).TakeWhile(element => element is not null).Select(element => RevitElement.Create(doc, element.UniqueId));
    }

    public static bool IsElementInView(this View v, Document doc, Element e)
    {
        // thanks to Colin Stark @ https://stackoverflow.com/questions/44012630/determine-is-a-familyinstance-is-visible-in-a-view
        var filter = ParameterFilterRuleFactory.CreateEqualsRule(new ElementId(BuiltInParameter.ID_PARAM), e.Id);
        var idFilter = new ElementParameterFilter(filter);
        var catFilter = new ElementCategoryFilter(v.Category.Id);
        return new FilteredElementCollector(doc, v.Id)
            .WhereElementIsNotElementType()
            .WherePasses(catFilter)
            .WherePasses(idFilter)
            .Any();
    }

    public static IEnumerable<string> GetElementsInViewByCategory(this View v, Document doc, Category categoryToFilter)
    {
        // thanks to Colin Stark @ https://stackoverflow.com/questions/44012630/determine-is-a-familyinstance-is-visible-in-a-view
        using var trans = new Transaction(doc);
        var catFilter = new ElementCategoryFilter(categoryToFilter.Id);
        
        return new FilteredElementCollector(doc, v.Id)
            .WhereElementIsNotElementType()
            .WherePasses(catFilter)
            .Select(x => x.UniqueId);
    }

    public static string ElementsOfCategoryInViewToJsonl(this View v, Document doc, Category categoryToFilter)
    {
        var sb = new StringBuilder();
        v.GetElementsInViewByCategory(doc, categoryToFilter)
            .Select(x => RevitElement.Create(doc, x))
            .Where(x => x is not null)
            .SelectMany(x => x!.Value.Parameters.Where(y => y is not null)
                .Select(y => y!.Value))
            .ToList()
            .ForEach(param => sb.AppendLine(JsonSerializer.Serialize(param)));
        return sb.ToString().TrimEnd(); 
    }

    
}