using Autodesk.Revit.DB;

namespace Direwolf.Definitions.RevitApi;

public readonly record struct ProjectSite(
    string PlaceName,
    double Elevation,
    double Latitude,
    double Longitude,
    double TimeZone,
    string GeoCoordinateSystemId,
    string GeoCoordinateSystemDefinition)
{
    public static ProjectSite Create(Document doc)
    {
        return new ProjectSite(doc.SiteLocation.PlaceName,
                               doc.SiteLocation.Elevation,
                               doc.SiteLocation.Latitude,
                               doc.SiteLocation.Longitude,
                               doc.SiteLocation.TimeZone,
                               doc.SiteLocation.GeoCoordinateSystemId,
                               doc.SiteLocation.GeoCoordinateSystemDefinition);
    }
}