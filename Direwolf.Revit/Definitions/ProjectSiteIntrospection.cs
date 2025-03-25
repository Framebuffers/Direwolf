using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct ProjectSiteIntrospection
    {
        public string? PlaceName { get; init; }
        public double? Elevation { get; init; }
        public double? Latitude { get; init; }
        public double? Longitude { get; init; }
        public double? TimeZone { get; init; }
        public string? GeoCoordinateSystemId { get; init; }
        public string? GeoCoordinateSystemDefinition { get; init; }

        public ProjectSiteIntrospection(Document document)
        {
            PlaceName = document.SiteLocation.PlaceName ?? string.Empty;
            Elevation = document.SiteLocation.Elevation;
            Latitude = document.SiteLocation.Latitude;
            Longitude = document.SiteLocation.Longitude;
            TimeZone = document.SiteLocation.TimeZone;
            GeoCoordinateSystemId = document.SiteLocation.GeoCoordinateSystemId ?? string.Empty;
            GeoCoordinateSystemDefinition = document.SiteLocation.GeoCoordinateSystemDefinition ?? string.Empty;
        }
        public ProjectSiteIntrospection() { }
    }

}





