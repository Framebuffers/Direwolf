using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions;

/// <summary>
///     Retrieves the Units set on a given Revit Document.
/// </summary>
/// <param name="Document">Revit Document</param>
public readonly record struct UnitIntrospection(Document Document)
{
    public string LengthUnits => Document.GetUnits().GetFormatOptions(SpecTypeId.Length).GetUnitTypeId().TypeId;
    public string AreaUnits => Document.GetUnits().GetFormatOptions(SpecTypeId.Area).GetUnitTypeId().TypeId;
    public string Angle => Document.GetUnits().GetFormatOptions(SpecTypeId.Angle).GetUnitTypeId().TypeId;
    public string Currency => Document.GetUnits().GetFormatOptions(SpecTypeId.Currency).GetUnitTypeId().TypeId;
    public string Number => Document.GetUnits().GetFormatOptions(SpecTypeId.Number).GetUnitTypeId().TypeId;

    public string RotationAngle =>
        Document.GetUnits().GetFormatOptions(SpecTypeId.RotationAngle).GetUnitTypeId().TypeId;

    public string SheetLength => Document.GetUnits().GetFormatOptions(SpecTypeId.SheetLength).GetUnitTypeId().TypeId;
    public string SiteAngle => Document.GetUnits().GetFormatOptions(SpecTypeId.SiteAngle).GetUnitTypeId().TypeId;
    public string Slope => Document.GetUnits().GetFormatOptions(SpecTypeId.Slope).GetUnitTypeId().TypeId;
    public string Speed => Document.GetUnits().GetFormatOptions(SpecTypeId.Speed).GetUnitTypeId().TypeId;
    public string Time => Document.GetUnits().GetFormatOptions(SpecTypeId.Time).GetUnitTypeId().TypeId;
    public string Volume => Document.GetUnits().GetFormatOptions(SpecTypeId.Volume).GetUnitTypeId().TypeId;
}