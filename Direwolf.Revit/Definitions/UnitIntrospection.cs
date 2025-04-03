using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions;

/// <summary>
///     Retrieves the Units set on a given Revit Document.
/// </summary>
/// <param name="Document">Revit Document</param>
public readonly record struct UnitIntrospection(Document Document)
{
    public string lengthUnits => Document.GetUnits().GetFormatOptions(SpecTypeId.Length).GetUnitTypeId().TypeId;
    public string areaUnits => Document.GetUnits().GetFormatOptions(SpecTypeId.Area).GetUnitTypeId().TypeId;
    public string angle => Document.GetUnits().GetFormatOptions(SpecTypeId.Angle).GetUnitTypeId().TypeId;
    public string currency => Document.GetUnits().GetFormatOptions(SpecTypeId.Currency).GetUnitTypeId().TypeId;
    public string number => Document.GetUnits().GetFormatOptions(SpecTypeId.Number).GetUnitTypeId().TypeId;

    public string rotationAngle =>
        Document.GetUnits().GetFormatOptions(SpecTypeId.RotationAngle).GetUnitTypeId().TypeId;

    public string sheetLength => Document.GetUnits().GetFormatOptions(SpecTypeId.SheetLength).GetUnitTypeId().TypeId;
    public string siteAngle => Document.GetUnits().GetFormatOptions(SpecTypeId.SiteAngle).GetUnitTypeId().TypeId;
    public string slope => Document.GetUnits().GetFormatOptions(SpecTypeId.Slope).GetUnitTypeId().TypeId;
    public string speed => Document.GetUnits().GetFormatOptions(SpecTypeId.Speed).GetUnitTypeId().TypeId;
    public string time => Document.GetUnits().GetFormatOptions(SpecTypeId.Time).GetUnitTypeId().TypeId;
    public string volume => Document.GetUnits().GetFormatOptions(SpecTypeId.Volume).GetUnitTypeId().TypeId;
}