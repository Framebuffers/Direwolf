using Autodesk.Revit.DB;
using Direwolf.Revit.Definitions;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls
{
    public record class GetDocumentIntrospection : RevitHowl
    {
        public GetDocumentIntrospection(Document doc)
        {
            ArgumentNullException.ThrowIfNull(doc);
            SetRevitDocument(doc);
        }
        private static string TryGetSafe(Func<string> func, string defaultValue)
        {
            try { return func(); } catch { return defaultValue; }
        }
        private static int TryGetSafe(Func<int> func, int defaultValue)
        {
            try { return func(); } catch { return defaultValue; }
        }
        private static double TryGetSafe(Func<double> func, double defaultValue)
        {
            try { return func(); } catch { return defaultValue; }
        }
        private static bool TryGetSafe(Func<bool> func, bool defaultValue)
        {
            try { return func(); } catch { return defaultValue; }
        }


        public override bool Execute()
        {
            try
            {
                var doc = GetRevitDocument();
                ProjectInformationIntrospection pji = new()
                {
                    projectName = TryGetSafe(() => doc.ProjectInformation.Name, string.Empty),
                    client = TryGetSafe(() => doc.ProjectInformation.ClientName,
                     string.Empty),
                    address = TryGetSafe(() => doc.ProjectInformation.Address, string.Empty),
                    author = TryGetSafe(() => doc.ProjectInformation.Author, string.Empty),
                    buildingName = TryGetSafe(() => doc.ProjectInformation.BuildingName, string.Empty),
                    issueDate = TryGetSafe(() => doc.ProjectInformation.IssueDate, string.Empty),
                    projectNumber = TryGetSafe(() => doc.ProjectInformation.Number, string.Empty),
                    organizationDescription = TryGetSafe(() => doc.ProjectInformation.OrganizationDescription, string.Empty),
                    organizationName = TryGetSafe(() => doc.ProjectInformation.OrganizationName, string.Empty),
                    status = TryGetSafe(() => doc.ProjectInformation.Status, string.Empty)
                };

                DocumentIntrospection d = new()
                {
                    documentName = TryGetSafe(() => doc.Title, string.Empty),
                    documentPath = TryGetSafe(() => doc.PathName, string.Empty),
                    documentUniqueId = TryGetSafe(() => doc.CreationGUID.ToString(), string.Empty),
                    documentVersionId = TryGetSafe(() => doc.ProjectInformation.VersionGuid.ToString(), string.Empty),
                    documentSaveCount = TryGetSafe(() => Document.GetDocumentVersion(doc).NumberOfSaves, 0),
                    warnings = [.. doc.GetWarnings().Select(x => x.GetDescriptionText())],
                    projectInformation = pji
                };

                var site = doc.SiteLocation;
                ProjectSiteIntrospection s = new()
                {
                    placeName = site.PlaceName,
                    elevation = site.Elevation,
                    latitude = site.Latitude,
                    longitude = site.Longitude,
                    timeZone = site.TimeZone,
                    geoCoordinateSystemId = site.GeoCoordinateSystemId,
                    geoCoordinateSystemDefinition = site.GeoCoordinateSystemDefinition
                };
                return true;
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Exception caught: {e.Message}"));
                return false;
            }
        }
    }
}
