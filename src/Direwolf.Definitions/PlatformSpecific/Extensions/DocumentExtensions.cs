using System.Runtime.Caching;
using Autodesk.Revit.DB;

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
    public static IEnumerable<CacheItem?> GetRevitDatabaseAsCacheItems(this Document doc)
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
                el = RevitElement.CreateAsCacheItem
                    (doc, element.UniqueId, out var _)
            })
            .Select
                (t => t.el);
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
    
}