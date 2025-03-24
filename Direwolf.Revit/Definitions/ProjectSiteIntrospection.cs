namespace Direwolf.Revit.Definitions
{
    public readonly record struct ProjectSiteIntrospection
    {
        public string? placeName { get; init; }
        public double? elevation { get; init; }
        public double? latitude { get; init; }
        public double? longitude { get; init; }
        public double? timeZone { get; init; }
        public string? geoCoordinateSystemId { get; init; }
        public string? geoCoordinateSystemDefinition { get; init; }
    }

}





