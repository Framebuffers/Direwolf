using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.PlatformSpecific;
using Direwolf.Definitions.PlatformSpecific.Extensions;

namespace Direwolf.Extensions;

public static class DatabaseExtensions
{
    /// <summary>
    ///     Creates a Dictionary of all <see cref="RevitElement" /> held inside the Revit <see cref="Document" />. This
    ///     requestType fetches all <see cref="Autodesk.Revit.DB.Element" /> bypassing the Cache. Therefore, it will
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
                (rvtElement!.Value.BuiltInCategory,
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
    public static Dictionary<BuiltInCategory, List<RevitElement?>>? GetElementsByCategory(this Document doc)
    {
        Dictionary<BuiltInCategory, List<RevitElement?>> categories = new();
        
        if (Direwolf.GetAllElements(doc, out var w) is not MessageResponse.Result) return null;
        foreach (var kvp in Wolfden.GetInstance(doc).GetRevitCache())
            try
            {
                var revitElement = (RevitElement)kvp.Value;
             
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
    ///     Gets the count of elements currently being held inside the <see cref="Direwolf" /> Wolfden.
    /// </summary>
    /// <param name="db">Direwolf instance</param>
    /// <param name="doc">Revit Document</param>
    /// <returns>The count of all <see cref="RevitElement" /> cached inside the Document's Wolfden.</returns>
    public static int GetDatabaseCount(this Document doc)
    {
        Direwolf.GetAllElements(doc, out var _);
        return Wolfden.GetInstance(doc).GetRevitCache().Count;
    }
}