using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct ProjectInformationIntrospection(Document Document)
    {
        public string projectName => Document.ProjectInformation.Name;
        public string client => Document.ProjectInformation.ClientName;
        public string address => Document.ProjectInformation.Address;
        public string author => Document.ProjectInformation.Author;
        public string buildingName => Document.ProjectInformation.BuildingName;
        public string issueDate => Document.ProjectInformation.IssueDate;
        public string location => Document.ProjectInformation.Location?.ToString() ?? string.Empty;
        public string projectNumber => Document.ProjectInformation.Number;
        public string organizationDescription => Document.ProjectInformation.OrganizationDescription;
        public string organizationName => Document.ProjectInformation.OrganizationName;
        public string status => Document.ProjectInformation.Status;
    }

}





