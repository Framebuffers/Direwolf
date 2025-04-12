using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions.Analysis;

/// <summary>
///     Retrieves information about the Site of a Revit Document.
/// </summary>
/// <param name="Document">Revit Document</param>
public readonly record struct ProjectSiteIntrospection(Document Document)
{
    public string PlaceName => Document.SiteLocation.PlaceName;
    public double Elevation => Document.SiteLocation.Elevation;
    public double Latitude => Document.SiteLocation.Latitude;
    public double Longitude => Document.SiteLocation.Longitude;
    public double TimeZone => Document.SiteLocation.TimeZone;
    public string GeoCoordinateSystemId => Document.SiteLocation.GeoCoordinateSystemId;
    public string GeoCoordinateSystemDefinition => Document.SiteLocation.GeoCoordinateSystemDefinition;
}