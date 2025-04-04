using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions;

/// <summary>
///     Information about the current Project held inside a Revit Document.
/// </summary>
/// <param name="Document">Revit Document</param>
public readonly record struct ProjectInformationIntrospection(Document Document)
{
    public string ProjectName => Document.ProjectInformation.Name;
    public string Client => Document.ProjectInformation.ClientName;
    public string Address => Document.ProjectInformation.Address;
    public string Author => Document.ProjectInformation.Author;
    public string BuildingName => Document.ProjectInformation.BuildingName;
    public string IssueDate => Document.ProjectInformation.IssueDate;
    public string Location => Document.ProjectInformation.Location?.ToString() ?? string.Empty;
    public string ProjectNumber => Document.ProjectInformation.Number;
    public string OrganizationDescription => Document.ProjectInformation.OrganizationDescription;
    public string OrganizationName => Document.ProjectInformation.OrganizationName;
    public string Status => Document.ProjectInformation.Status;
}