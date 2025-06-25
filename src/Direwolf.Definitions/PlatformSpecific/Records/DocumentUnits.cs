using System.Reflection;
using Autodesk.Revit.DB;
using static System.String;


namespace Direwolf.Definitions.PlatformSpecific.Records;

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
    public static bool IsInternal(ForgeTypeId unitTypeId)
    {
        PropertyInfo[] propertyInfos;
        propertyInfos = typeof(SpecTypeId).GetProperties(BindingFlags.Public | BindingFlags.Static) ;
        Array.Sort(propertyInfos,
            (PropertyInfoA, PropertyInfoB) => Compare(PropertyInfoA.Name, PropertyInfoB.Name, StringComparison.Ordinal));

        foreach (var property in propertyInfos)
            if (property.Name == unitTypeId.TypeId.GetType().Name)
                return true;
        return false;
        // return unitTypeId == SpecTypeId.Length ||
        //    unitTypeId == SpecTypeId.Area ||
        //    unitTypeId == SpecTypeId.Volume ||
        //    unitTypeId == SpecTypeId.Angle ||
        //    unitTypeId == SpecTypeId.Force ||
        //    unitTypeId == SpecTypeId.Mass ||
        //    unitTypeId == SpecTypeId.MassPerUnitLength ||
        //    unitTypeId == SpecTypeId.MassPerUnitArea ||
        //    unitTypeId == SpecTypeId.HvacDensity ||
        //    unitTypeId == SpecTypeId.MassDensity ||
        //    unitTypeId == SpecTypeId.PipingDensity ||
        //    unitTypeId == SpecTypeId.AirFlowDensity ||
        //    unitTypeId == SpecTypeId.ApparentPowerDensity ||
        //    unitTypeId == SpecTypeId.ElectricalPowerDensity ||
        //    unitTypeId == SpecTypeId.HvacPowerDensity ||
        //    unitTypeId == SpecTypeId.Speed ||
        //    unitTypeId == SpecTypeId.Flow ||
        //    unitTypeId == SpecTypeId.PipingPressure ||
        //    unitTypeId == SpecTypeId.HvacPressure ||
        //    unitTypeId == SpecTypeId.Energy ||
        //    unitTypeId == SpecTypeId.Power ||
        //    unitTypeId == SpecTypeId.HeatGain ||
        //    unitTypeId == SpecTypeId.Temperature ||
        //    unitTypeId == SpecTypeId.Luminance ||
        //    unitTypeId == SpecTypeId.Illuminance ||
        //    unitTypeId == SpecTypeId.ElectricalCurrent ||
        //    unitTypeId == SpecTypeId.ElectricalPotential ||
        //    unitTypeId == SpecTypeId.ElectricalFrequency ||
        //    unitTypeId == SpecTypeId.ElectricalPower ||
        //    unitTypeId == SpecTypeId.ElectricalPowerDensity ||
        //    unitTypeId == SpecTypeId.ReinforcementArea ||
        //    unitTypeId == SpecTypeId.ReinforcementLength ||
        //    unitTypeId == SpecTypeId.ReinforcementVolume ||
        //    unitTypeId == SpecTypeId.ReinforcementSpacing ||
        //    unitTypeId == SpecTypeId.ReinforcementCover ||
        //    unitTypeId == SpecTypeId.BarDiameter ||
        //    unitTypeId == SpecTypeId.CrackWidth ||
        //    unitTypeId == SpecTypeId.SectionDimension ||
        //    unitTypeId == SpecTypeId.SectionProperty ||
        //    unitTypeId == SpecTypeId.SectionModulus ||
        //    unitTypeId == SpecTypeId.MomentOfInertia ||
        //    unitTypeId == SpecTypeId.WarpingConstant ||
        //    unitTypeId == SpecTypeId.MassPerUnitLength ||
        //    unitTypeId == SpecTypeId.WeightPerUnitLength ||
        //    unitTypeId == SpecTypeId.SurfaceAreaPerUnitLength ||
        //    unitTypeId == SpecTypeId.PipeDimension ||
        //    unitTypeId == SpecTypeId.PipeInsulationThickness ||
        //    unitTypeId == SpecTypeId.PipingMass ||
        //    unitTypeId == SpecTypeId.PipeSize ||
        //    unitTypeId == SpecTypeId.HvacDensity ||
        //    unitTypeId == SpecTypeId.HvacEnergy ||
        //    unitTypeId == SpecTypeId.HvacFriction ||
        //    unitTypeId == SpecTypeId.HvacPower ||
        //    unitTypeId == SpecTypeId.HvacPowerDensity ||
        //    unitTypeId == SpecTypeId.HvacPressure ||
        //    unitTypeId == SpecTypeId.HvacTemperature ||
        //    unitTypeId == SpecTypeId.HvacVelocity ||
        //    unitTypeId == SpecTypeId.AirFlow ||
        //    unitTypeId == SpecTypeId.DuctSize ||
        //    unitTypeId == SpecTypeId.CrossSection ||
        //    unitTypeId == SpecTypeId.HeatGain ||
        //    unitTypeId == SpecTypeId.CableTraySize ||
        //    unitTypeId == SpecTypeId.ConduitSize ||
        //    unitTypeId == SpecTypeId.DemandFactor ||
        //    unitTypeId == SpecTypeId.ApparentPowerDensity ||
        //    unitTypeId == SpecTypeId.LuminousFlux ||
        //    unitTypeId == SpecTypeId.LuminousIntensity ||
        //    unitTypeId == SpecTypeId.Efficacy ||
        //    unitTypeId == SpecTypeId.ColorTemperature;
    }
    
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