using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct ProjectInformationIntrospection(Document document)
    {
        public string projectName
        {
            get
            {
                try
                {

                    return document.ProjectInformation.Name;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        public string client
        {
            get
            {
                try
                {
                    return document.ProjectInformation.ClientName;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        public string address
        {
            get
            {
                try
                {
                    return document.ProjectInformation.Address;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        public string author
        {
            get
            {
                try
                {
                    return document.ProjectInformation.Author;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        public string buildingName
        {
            get
            {
                try
                {

                    return document.ProjectInformation.BuildingName;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        public string issueDate
        {
            get
            {
                try
                {
                    return document.ProjectInformation.IssueDate;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        public string location
        {
            get
            {
                try
                {
                    return document.ProjectInformation.Location?.ToString() ?? string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }

        }
        public string projectNumber
        {
            get
            {
                try
                {
                    return document.ProjectInformation.Number;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        public string organizationDescription
        {
            get
            {
                try
                {
                    return document.ProjectInformation.OrganizationDescription;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        public string organizationName
        {
            get
            {
                try
                {
                    return document.ProjectInformation.OrganizationName;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        public string status
        {
            get
            {
                try
                {
                    return document.ProjectInformation.Status;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
    }

}





