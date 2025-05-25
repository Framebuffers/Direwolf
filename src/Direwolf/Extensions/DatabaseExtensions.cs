using System.Text.Json;

using Autodesk.Revit.DB;

using Direwolf.Definitions.RevitApi;

namespace Direwolf.Extensions;

public static class DatabaseExtensions
{
    /// <summary>
    /// Creates a Dictionary of <see cref="RevitElement"/> sorted by <see cref="Autodesk.Revit.DB.BuiltInCategory"/>.
    /// This can be used to create a snapshot of a Revit file and all its most important data, including parameters.
    /// </summary>
    ///
    /// <remarks>
    /// This dictionary only contains categories with valid elements present in the currently-loaded
    /// <see cref="Document"/>.
    /// </remarks>
    /// 
    /// <returns>A dictionary with a <see cref="Autodesk.Revit.DB.BuiltInCategory"/> as a key, and a list of
    /// <see cref="RevitElement"/> internal definitions as values.
    /// </returns>
    public static Dictionary<string, object> GetElementsByCategory(
        this Direwolf db)
    {
        _ = db ?? throw new ArgumentNullException(nameof(db));

        return Direwolf.ElementCache.ToDictionary(x => x.Key,
                                                  x => x.Value);
    }

    public static string Debug_GetDatabaseReport(this Direwolf db)
    {
        var e = db.GetElementsByCategory();

        // transaction DB count by ElementID
        var d = new Dictionary<string, object>()
        {
            [db._document.Title] = new Dictionary<string, object>()
            {
                ["ElementCache"] = new Dictionary<string, object>()
                {
                    ["Count"]
                        = Direwolf.ElementCache.Count().ToString(),
                    ["Stats"] = e.Select(x => x.Value)
                }
            }
        };

        return JsonSerializer.Serialize(d);
    }

    public static int GetDatabaseCount(this Direwolf db)
    {
        _ = db ?? throw new ArgumentNullException(nameof(db));

        return Direwolf.ElementCache.Count();
    }
}