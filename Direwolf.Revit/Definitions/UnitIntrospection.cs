using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct UnitIntrospection
    {
        public string? LengthUnits { get; init; }
        public string? AreaUnits { get; init; }
        public string? Angle { get; init; }
        public string? Currency { get; init; }
        public string? Number { get; init; }
        public string? RotationAngle { get; init; }
        public string? SheetLength { get; init; }
        public string? SiteAngle { get; init; }
        public string? Slope { get; init; }
        public string? Speed { get; init; }
        public string? Time { get; init; }
        public string? Volume { get; init; }

        public UnitIntrospection(Document document)
        {
            LengthUnits = document.GetUnits().GetFormatOptions(SpecTypeId.Length).GetUnitTypeId().TypeId;
            AreaUnits = document.GetUnits().GetFormatOptions(SpecTypeId.Area).GetUnitTypeId().TypeId;
            Angle = document.GetUnits().GetFormatOptions(SpecTypeId.Angle).GetUnitTypeId().TypeId;
            Currency = document.GetUnits().GetFormatOptions(SpecTypeId.Currency).GetUnitTypeId().TypeId;
            Number = document.GetUnits().GetFormatOptions(SpecTypeId.Number).GetUnitTypeId().TypeId;
            RotationAngle = document.GetUnits().GetFormatOptions(SpecTypeId.RotationAngle).GetUnitTypeId().TypeId;
            SheetLength = document.GetUnits().GetFormatOptions(SpecTypeId.SheetLength).GetUnitTypeId().TypeId;
            SiteAngle = document.GetUnits().GetFormatOptions(SpecTypeId.SiteAngle).GetUnitTypeId().TypeId;
            Slope = document.GetUnits().GetFormatOptions(SpecTypeId.Slope).GetUnitTypeId().TypeId;
            Speed = document.GetUnits().GetFormatOptions(SpecTypeId.Speed).GetUnitTypeId().TypeId;
            Time = document.GetUnits().GetFormatOptions(SpecTypeId.Time).GetUnitTypeId().TypeId;
            Volume = document.GetUnits().GetFormatOptions(SpecTypeId.Volume).GetUnitTypeId().TypeId;
        }
        public UnitIntrospection() { }
    }
}
