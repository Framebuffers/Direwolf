using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct UnitIntrospection(Document document)
    {
        public string lengthUnits => document.GetUnits().GetFormatOptions(SpecTypeId.Length).GetUnitTypeId().TypeId;
        public string areaUnits => document.GetUnits().GetFormatOptions(SpecTypeId.Area).GetUnitTypeId().TypeId;
        public string angle => document.GetUnits().GetFormatOptions(SpecTypeId.Angle).GetUnitTypeId().TypeId;
        public string currency => document.GetUnits().GetFormatOptions(SpecTypeId.Currency).GetUnitTypeId().TypeId;
        public string number => document.GetUnits().GetFormatOptions(SpecTypeId.Number).GetUnitTypeId().TypeId;
        public string rotationAngle => document.GetUnits().GetFormatOptions(SpecTypeId.RotationAngle).GetUnitTypeId().TypeId;
        public string sheetLength => document.GetUnits().GetFormatOptions(SpecTypeId.SheetLength).GetUnitTypeId().TypeId;
        public string siteAngle => document.GetUnits().GetFormatOptions(SpecTypeId.SiteAngle).GetUnitTypeId().TypeId;
        public string slope => document.GetUnits().GetFormatOptions(SpecTypeId.Slope).GetUnitTypeId().TypeId;
        public string speed => document.GetUnits().GetFormatOptions(SpecTypeId.Speed).GetUnitTypeId().TypeId;
        public string time => document.GetUnits().GetFormatOptions(SpecTypeId.Time).GetUnitTypeId().TypeId;
        public string volume => document.GetUnits().GetFormatOptions(SpecTypeId.Volume).GetUnitTypeId().TypeId;
    }

}





