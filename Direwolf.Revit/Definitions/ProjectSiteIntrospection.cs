using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct ProjectSiteIntrospection(Document Document)
    {
        public string placeName => Document.SiteLocation.PlaceName;
        public double elevation => Document.SiteLocation.Elevation;
        public double latitude => Document.SiteLocation.Latitude;
        public double longitude => Document.SiteLocation.Longitude;
        public double timeZone => Document.SiteLocation.TimeZone;
        public string geoCoordinateSystemId => Document.SiteLocation.GeoCoordinateSystemId;
        public string geoCoordinateSystemDefinition => Document.SiteLocation.GeoCoordinateSystemDefinition;
    }

}





