using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct ProjectInformationIntrospection
    {
        public string? ProjectName { get; init; }
        public string? Client { get; init; }
        public string? Address { get; init; }
        public string? Author { get; init; }
        public string? BuildingName { get; init; }
        public string? IssueDate { get; init; }
        public string? Location { get; init; }
        public string? ProjectNumber { get; init; }
        public string? OrganizationDescription { get; init; }
        public string? OrganizationName { get; init; }
        public string? Status { get; init; }

        public ProjectInformationIntrospection(Document document)
        {
            ProjectName = document.ProjectInformation.Name ?? string.Empty;
            Client = document.ProjectInformation.ClientName ?? string.Empty;
            Address = document.ProjectInformation.Address ?? string.Empty;
            Author = document.ProjectInformation.Author ?? string.Empty;
            BuildingName = document.ProjectInformation.BuildingName ?? string.Empty;
            IssueDate = document.ProjectInformation.IssueDate ?? string.Empty;
            Location = document.ProjectInformation.Location?.ToString() ?? string.Empty;
            ProjectNumber = document.ProjectInformation.Number ?? string.Empty;
            OrganizationDescription = document.ProjectInformation.OrganizationDescription ?? string.Empty;
            OrganizationName = document.ProjectInformation.OrganizationName ?? string.Empty;
            Status = document.ProjectInformation.Status ?? string.Empty;
        }

        public ProjectInformationIntrospection() { }
    }

}





