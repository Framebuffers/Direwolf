namespace Direwolf.Database.Tokens;

public readonly record struct ProjectInformation(
    string ProjectName,
    string Client,
    string Address,
    string Author,
    string BuildingName,
    string IssueDate,
    string Location,
    string ProjectNumber,
    string OrganizationDescription,
    string OrganizationName,
    string Status)
{
    public static ProjectInformation Create(Autodesk.Revit.DB.Document doc) =>
        new(doc.ProjectInformation.Name,
            doc.ProjectInformation.ClientName,
            doc.ProjectInformation.Address,
            doc.ProjectInformation.Author,
            doc.ProjectInformation.BuildingName,
            doc.ProjectInformation.IssueDate,
            doc.ProjectInformation.Location.ToString() ?? string.Empty,
            doc.ProjectInformation.Number,
            doc.ProjectInformation.OrganizationDescription,
            doc.ProjectInformation.OrganizationName,
            doc.ProjectInformation.Status);
}