using System.Runtime.Caching;
using System.Text.Json;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.RevitApi;
using ArgumentNullException = System.ArgumentNullException;

namespace Direwolf.Extensions;

public static class DatabaseExtensions
{
    /// <summary>
    ///     Creates a Dictionary of <see cref="RevitElement" /> sorted by
    ///     <see cref="Autodesk.Revit.DB.BuiltInCategory" />.
    ///     This can be used to create a snapshot of a Revit file and all its most
    ///     important data, including parameters.
    /// </summary>
    /// <remarks>
    ///     This dictionary only contains categories with valid elements present in the
    ///     currently-loaded
    ///     <see cref="Document" />.
    /// </remarks>
    /// <returns>
    ///     A dictionary with a <see cref="Autodesk.Revit.DB.BuiltInCategory" /> as a
    ///     key, and a list of
    ///     <see cref="RevitElement" /> internal definitions as values.
    /// </returns>
    public static ObjectCache GetElementCache(this Direwolf? db)
    {
        _ = db
            ?? throw new ArgumentNullException
                (nameof(db));

        return Direwolf.ElementCache;
    }

    /// <summary>
    ///     Creates a Dictionary of all <see cref="RevitElement" /> held inside the Revit <see cref="Document" />. This
    ///     method fetches all <see cref="Autodesk.Revit.DB.Element" /> bypassing the Cache. Therefore, it will
    ///     always reflect the state of the Document at runtime, regardless of the state Direwolf is.
    /// </summary>
    /// <param name="doc">Revit Document</param>
    /// <returns>
    ///     A dictionary with all <see cref="RevitElement" /> records, sorted by their
    ///     <see cref="Autodesk.Revit.DB.BuiltInCategory" />
    /// </returns>
    public static Dictionary<BuiltInCategory, List<RevitElement?>> GetRevitDbByCategory(this Document doc)
    {
        Dictionary<BuiltInCategory, List<RevitElement?>> categories = new();
        foreach (var rvtElement in doc.GetRevitDatabase())
        {
            if (!categories.TryGetValue
                (rvtElement.Value.BuiltInCategory,
                    out var elementList))
            {
                elementList = [];
                categories.Add
                (rvtElement.Value.BuiltInCategory,
                    elementList);
            }

            elementList.Add
                (rvtElement);
        }

        return categories;
    }

    /// <summary>
    ///     Creates a Dictionary of all <see cref="RevitElement" /> held inside the <see cref="Direwolf" /> Wolfden.
    ///     It reflects the state of the Direwolf cache at the moment of capture.
    /// </summary>
    /// <param name="doc">Revit Document</param>
    /// <returns>
    ///     A dictionary with all <see cref="RevitElement" /> records, sorted by their
    ///     <see cref="Autodesk.Revit.DB.BuiltInCategory" />
    /// </returns>
    public static Dictionary<BuiltInCategory, List<RevitElement?>> GetCacheByCategory(this Document doc)
    {
        Dictionary<BuiltInCategory, List<RevitElement?>> categories = new();
        foreach (var kvp in Direwolf.GetDatabase
                         (doc)
                     ?.GetElementCache()!)
            try
            {
                if (kvp.Value is not RevitElement revitElement)
                    continue;
                if (!categories.TryGetValue
                    (revitElement.BuiltInCategory,
                        out var categoryList))
                {
                    categoryList = [];
                    categories.Add
                    (revitElement.BuiltInCategory,
                        categoryList);
                }

                categoryList.Add
                    (revitElement);
            }
            catch (InapplicableDataException e)
            {
                _ = e;
            }

        return categories;
    }

    /// <summary>
    ///     Returns a serialized JSON string containing the whole Revit DB serialized as a Dictionary of
    ///     <see cref="RevitElement" />
    ///     sorted by their <see cref="Autodesk.Revit.DB.BuiltInCategory" />. This bypasses the <see cref="Direwolf" />
    ///     Wolfden.
    /// </summary>
    /// <param name="doc">Revit Document</param>
    /// <returns>
    ///     A JSON string containing the <see cref="RevitParameter" /> of all <see cref="RevitElement" /> inside the
    ///     <see cref="Document" />, sorted by <see cref="Autodesk.Revit.DB.BuiltInCategory" />
    /// </returns>
    public static string RevitDbAsString(this Document doc)
    {
        var dict = doc.GetRevitDbByCategory();
        var d = new Dictionary<string, object>
        {
            [doc.Title] = new Dictionary<string, object>
            {
                ["Wolfden"] = new Dictionary<string, object>
                {
                    ["Count"] = Direwolf.ElementCache.Count().ToString(),
                    ["Stats"] = dict.Select
                        (x => x.Value.Count)
                }
            }
        };

        return JsonSerializer.Serialize
            (d);
    }

    /// <summary>
    ///     Gets the count of elements currently being held inside the <see cref="Direwolf" /> Wolfden.
    /// </summary>
    /// <param name="db">Direwolf instance</param>
    /// <returns>The count of all <see cref="RevitElement" /> cached inside Direwolf.</returns>
    public static int GetDatabaseCount(this Direwolf db)
    {
        _ = db;
        return Direwolf.ElementCache.Count();
    }
}