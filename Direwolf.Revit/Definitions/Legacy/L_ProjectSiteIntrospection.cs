using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions.Legacy
{
    public readonly record struct L_ProjectSiteIntrospection
    {
        public string? PlaceName { get; init; }
        public double? Elevation { get; init; }
        public double? Latitude { get; init; }
        public double? Longitude { get; init; }
        public double? TimeZone { get; init; }
        public string? GeoCoordinateSystemId { get; init; }
        public string? GeoCoordinateSystemDefinition { get; init; }

        public L_ProjectSiteIntrospection(Document document)
        {
            PlaceName = document.SiteLocation.PlaceName ?? string.Empty;
            Elevation = document.SiteLocation.Elevation;
            Latitude = document.SiteLocation.Latitude;
            Longitude = document.SiteLocation.Longitude;
            TimeZone = document.SiteLocation.TimeZone;
            GeoCoordinateSystemId = document.SiteLocation.GeoCoordinateSystemId ?? string.Empty;
            GeoCoordinateSystemDefinition = document.SiteLocation.GeoCoordinateSystemDefinition ?? string.Empty;
        }
        public L_ProjectSiteIntrospection() { }

        public static string AsSql()
        {
            return """
                INSERT INTO "SiteInformation"(
                "documentId",
                "document",
                "placeName",
                "elevation",
                "latitude",
                "longitude",
                "timeZone",
                "coordinatedSystemId",
                "coordinateSystemDefinition"
                ) VALUES (
                @documentId,
                @document,
                @placeName,
                @elevation,
                @latitude,
                @longitude,
                @timeZone,
                @coordinatedSystemId,
                @coordinateSystemDefinition);
                """;
        }
    }

}





