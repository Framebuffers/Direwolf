using System.Globalization;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;

namespace Direwolf.Definitions.PlatformSpecific;

/// <summary>
///     A symbolic representation of a <see cref="Parameter" /> inside a <see cref="Autodesk.Revit.DB.Element" />. It
///     contains only three properties:
///     type, key and value. Using these three combined, a <see cref="Parameter" /> can be imported and exported to and
///     from Revit safely.
/// </summary>
/// <param name="Key">The <see cref="Definition.Name" /> property of a give Element.</param>
/// <param name="Value">The Value held inside a given <see cref="Parameter" />.</param>
public readonly record struct RevitParameter(StorageType StorageType, string Key, string Value)
{
    /// <summary>
    ///     Generate a <see cref="RevitParameter" /> from a <see cref="Parameter" />.
    /// </summary>
    /// <param name="p">A <see cref="Parameter" /> coming from a <see cref="Autodesk.Revit.DB.Element" />.</param>
    /// <returns>
    ///     A <see cref="RevitElement" /> holding: its <see cref="Definition.Name" />, <see cref="Parameter" /> value and
    ///     <see cref="StorageType" />.
    /// </returns>
    public static RevitParameter? Create(Parameter p)
    {
        try
        {
            return new RevitParameter(
                p.StorageType,
                p.Definition.Name,
                GetValue
                    (p));
        }
        catch
        {
            return null;
        }
    }

    private static string GetValue(Parameter p)
    {
        try
        {
            return p.StorageType switch
            {
                StorageType.None => "None",
                StorageType.Integer => p.AsInteger().ToString(),
                StorageType.Double => p.AsDouble()
                    .ToString
                        (CultureInfo.InvariantCulture),
                StorageType.String => p.AsString(),
                StorageType.ElementId => p.AsElementId().ToString(),
                _ => "None"
            };
        }
        catch
        {
            return "undefined";
        }

    }
}