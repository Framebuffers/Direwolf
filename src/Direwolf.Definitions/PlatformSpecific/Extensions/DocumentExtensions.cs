using System.Runtime.Caching;
using System.Threading.Tasks.Sources;
using Autodesk.Revit.DB;
using Direwolf.Definitions.Extensions;

namespace Direwolf.Definitions.PlatformSpecific.Extensions;

/// <summary>
///     Helper functions to manage data in and out of a <see cref="Document" />
/// </summary>
public static class DocumentExtensions
{
    /// <summary>
    ///     Returns every single valid <see cref="Autodesk.Revit.DB.Element" /> inside the <see cref="Document" /> as a
    ///     <see cref="RevitElement" />.
    /// </summary>
    /// <param name="doc">Revit Document</param>
    /// <returns>
    ///     A collection containing the Direwolf-formatted representation of all the valid
    ///     <see cref="Autodesk.Revit.DB.Element" /> inside the Revit <see cref="Document" />.
    /// </returns>
    public static IEnumerable<RevitElement?> GetRevitDatabase(this Document doc)
    {
        ArgumentNullException.ThrowIfNull
            (doc);

        using var filteredElementCollector = new FilteredElementCollector(doc);
        return filteredElementCollector
            .WhereElementIsNotElementType()
            .WhereElementIsViewIndependent()
            .ToElements()
            .Where(x => x.Id != ElementId.InvalidElementId)
            .Select
            (element => new
            {
                el = RevitElement.Create
                    (doc, element.UniqueId)
            })
            .Select
                (t => t.el);
    }

    /// <summary>
    ///     Returns the <see cref="Autodesk.Revit.DB.ElementId" /> of every single valid
    ///     <see cref="Autodesk.Revit.DB.ElementType" /> inside the <see cref="Document" />.
    /// </summary>
    /// <param name="doc">Revit Document</param>
    /// <returns>
    ///     A collection of all the <see cref="Autodesk.Revit.DB.ElementId" /> for every valid
    ///     <see cref="Autodesk.Revit.DB.ElementType" /> inside the Revit <see cref="Document" />.
    /// </returns>
    public static IEnumerable<ElementId> GetElementTypes(this Document doc)
    {
        using var filteredElementCollector = new FilteredElementCollector(doc);
        return from elementId in filteredElementCollector
                .WhereElementIsElementType()
                .ToElementIds()
            select elementId;
    }

    /// <summary>
    ///     Returns the current state of the <see cref="Document" /> as <see cref="CacheItem" /> to be used inside the
    ///     <see cref="Direwolf" /> ElementCache.
    /// </summary>
    /// <param name="doc">Revit Document</param>
    /// <returns>
    ///     A collection of every single valid <see cref="Autodesk.Revit.DB.Element" /> inside the <see cref="Document" />
    ///     as a <see cref="CacheItem" /> of a <see cref="RevitElement" />.
    /// </returns>
    public static IEnumerable<CacheItem?> GetRevitDatabaseAsCacheItems(this Document doc, out Dictionary<string, string?> keyCacheValue)
    {
        ArgumentNullException.ThrowIfNull
            (doc);
        var x =new FilteredElementCollector(doc)
            .WhereElementIsNotElementType()
            .WhereElementIsViewIndependent()
            .ToElements()
            .Where(x => x.Id != ElementId.InvalidElementId)
            .Select(element => new
            {
                el = RevitElement.CreateAsCacheItem(doc, element.UniqueId, out var rvt),
                elementUniqueId = rvt!.Value.ElementUniqueId,
                direwolfId = rvt.Value.Id.Value 
            }).ToList();
        
        var elements = x.Select(x => x.el).ToList();
        var revitUnique = x.Select(x => x.elementUniqueId).ToList();
        var direwolfUnique = x.Select(x => x.direwolfId).ToList();

        keyCacheValue = revitUnique.Zip(direwolfUnique).ToDictionary(y => y.First, y => y.Second);
        return elements;
        // var d = filteredElementCollector
        //     .WhereElementIsNotElementType()
        //     .WhereElementIsViewIndependent()
        //     .ToElements()
        //     .Where(x => x.Id != ElementId.InvalidElementId)
        //     .Select
        //     (element => new
        //     {
        //         el = RevitElement.CreateAsCacheItem
        //             (doc, element.UniqueId, out var values),
        //         values
        //
        //     })
        //     .Select
        //         (t => (t.el, t.values));
        //     unique =  d.Select(x => x.values).ToList();
        //     return d.Select(x => x.el).ToList();
    }

    internal static IEnumerable<Element?> GetAllElementsFromDocument(this Document doc)
    {
        ArgumentNullException.ThrowIfNull(doc);

        using var filteredElementCollector = new FilteredElementCollector(doc);
        return filteredElementCollector
            .WhereElementIsNotElementType()
            .WhereElementIsViewIndependent()
            .ToElements();
    }
    
    public static IDictionary<string, object> GetElementsBelongingToRevitDocument(this Document doc, IDictionary<string, object> cache)
    {
        var signature = doc.GetDocumentVersionHash();
        var dict = new Dictionary<string, object>();
        foreach (var element in cache)
        {
            var id = element.Key.ParseAsCuid();
            if(id.Value is null) throw new InvalidDataException();
            var concatVersionSignature = string.Concat(id.Counter, id.Fingerprint); // this is how Revit elements are told apart
            if (signature != concatVersionSignature) continue;
            dict[element.Key] = element.Value;
        }
        return dict;
    }
    
}