using Autodesk.Revit.DB;

namespace Direwolf.Definitions.RevitApi;

// Unimplemented feature as of 2025-05-29
public readonly record struct DocumentUnits(
    string Volume,
    string LengthUnits,
    string AreaUnits,
    string Angle,
    string Currency,
    string Number,
    string RotationAngle,
    string SheetLength,
    string SiteAngle,
    string Slope,
    string Speed,
    string Time)
{
    public static DocumentUnits Create(Document document)
    {
        return new DocumentUnits(document.GetUnits()
                .GetFormatOptions
                    (SpecTypeId.Volume)
                .GetUnitTypeId()
                .TypeId,
            document.GetUnits()
                .GetFormatOptions
                    (SpecTypeId.Length)
                .GetUnitTypeId()
                .TypeId,
            document.GetUnits()
                .GetFormatOptions
                    (SpecTypeId.Area)
                .GetUnitTypeId()
                .TypeId,
            document.GetUnits()
                .GetFormatOptions
                    (SpecTypeId.Angle)
                .GetUnitTypeId()
                .TypeId,
            document.GetUnits()
                .GetFormatOptions
                    (SpecTypeId.Currency)
                .GetUnitTypeId()
                .TypeId,
            document.GetUnits()
                .GetFormatOptions
                    (SpecTypeId.Number)
                .GetUnitTypeId()
                .TypeId,
            document.GetUnits()
                .GetFormatOptions
                    (SpecTypeId.RotationAngle)
                .GetUnitTypeId()
                .TypeId,
            document.GetUnits()
                .GetFormatOptions
                    (SpecTypeId.SheetLength)
                .GetUnitTypeId()
                .TypeId,
            document.GetUnits()
                .GetFormatOptions
                    (SpecTypeId.SiteAngle)
                .GetUnitTypeId()
                .TypeId,
            document.GetUnits()
                .GetFormatOptions
                    (SpecTypeId.Slope)
                .GetUnitTypeId()
                .TypeId,
            document.GetUnits()
                .GetFormatOptions
                    (SpecTypeId.Speed)
                .GetUnitTypeId()
                .TypeId,
            document.GetUnits()
                .GetFormatOptions
                    (SpecTypeId.Time)
                .GetUnitTypeId()
                .TypeId);
    }
}