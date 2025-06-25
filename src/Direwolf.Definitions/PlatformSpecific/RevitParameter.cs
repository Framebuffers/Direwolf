using System.Diagnostics;
using System.Globalization;
using Autodesk.Revit.DB;

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
        // as simple an approach as possible.
        // ConvertFromInternalUnits() always takes doubles as an input.
        // elements without a 
        if (p.GetUnitTypeId() is not null && p.StorageType == StorageType.Double)
        {
            try
            {
                if (UnitUtils.IsUnit(p.Definition.GetDataType()))
                    Debug.Print($"Unit converted: {p.Definition.Name}, {p.GetTypeId().TypeId}");
                 return UnitUtils.ConvertFromInternalUnits(p.AsDouble(),
                        p.GetUnitTypeId())
                    .ToString(CultureInfo.InvariantCulture);
            }
            catch
            {
                return p.AsDouble().ToString(CultureInfo.InvariantCulture);
            } 
        }
        
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
}