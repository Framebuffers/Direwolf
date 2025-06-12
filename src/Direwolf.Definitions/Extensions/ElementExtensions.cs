using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Direwolf.Definitions.RevitApi;
using static Direwolf.Definitions.Extensions.DirewolfExtensions;

namespace Direwolf.Definitions.Extensions;

/// <summary>
///     Helper methods for <see cref="Autodesk.Revit.DB.Element" />.
/// </summary>
public static class ElementExtensions
{
    /// <summary>
    ///     Safely attempts to get the <see cref="RevitParameter" /> of a given <see cref="Autodesk.Revit.DB.Element" />,
    ///     handling any Exception being thrown by Revit.
    /// </summary>
    /// <param name="element">A Revit Element.</param>
    /// <param name="parameters">
    ///     A list containing all the <see cref="RevitParameter" /> obtained from the
    ///     <see cref="Autodesk.Revit.DB.Element" />.
    /// </param>
    /// <returns>True if the operation succeeded. False if otherwise.</returns>
    public static void TryGetParameters(this Element element, out List<RevitParameter?> parameters)
    {
        try
        {
            parameters = [];
            if (element.Category is null || element.Document is null || element.Parameters is null) return;
            parameters = [];
            foreach (Parameter p in element.Parameters)
            {
                if (p is null || !p.HasValue || p.Definition is null) continue;
                parameters.Add(TryDo(() => RevitParameter.Create(p)));
            }

            parameters.AddRange(from Parameter p in element.Parameters select RevitParameter.Create(p));
        }
        catch (Exception e)
        {
            Debug.Print(e.Message);
            parameters = [];
        }
    }

    public static void TryGetParameter(this Element element, BuiltInParameter bip, out RevitParameter? parameter)
    {
        try
        {
            parameter = null;
            if (element.Category is null || element.Document is null || element.Parameters is null) return;
            foreach (Parameter p in element.Parameters)
            {
                if (p is null || !p.HasValue || p.Definition is null) continue;
                try
                {
                    parameter = RevitParameter.Create(element.get_Parameter(bip));
                }
                catch (InapplicableDataException e)
                {
                    Debug.Print($"Hit {e.Message} on {element.UniqueId}");
                    _ = e; // this is to handle document is not workset-enabled errors.
                }
            }
        }
        catch (Exception e)
        {
            Debug.Print(e.Message);
            parameter = null;
        }
    }

    public static void TryParseParameterStringName(this Element element, string s, out RevitParameter? parameter)
    {
        parameter = null;
        if (element.Category is null || element.Document is null || element.Parameters is null) return;
        parameter = TryDo(() =>
            Enum.TryParse<BuiltInParameter>(s, true, out var builtInParameter)
                ? RevitParameter.Create(element.get_Parameter(builtInParameter))
                : null);
    }

   

    public static (Element? Element, ElementId? ElementId, Type? ElementType, ElementId? ElementTypeId) DeconstructElementUniqueId(this string elementUniqueId, Document doc)
        {
            var element = doc.GetElement(elementUniqueId);
            return element is null ? (null, null, null, null) 
                : (element, element.Id, TryDo(() => element.GetType()), element?.GetTypeId());
        } 
}