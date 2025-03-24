namespace Direwolf.Revit.Definitions
{
    public readonly record struct ProjectInformationIntrospection
    {
        public string? projectName { get; init; }
        public string? client { get; init; }
        public string? address { get; init; }
        public string? author { get; init; }
        public string? buildingName { get; init; }
        public string? issueDate { get; init; }
        public string? projectNumber { get; init; }
        public string? organizationDescription { get; init; }
        public string? organizationName { get; init; }
        public string? status { get; init; }
    }
}
