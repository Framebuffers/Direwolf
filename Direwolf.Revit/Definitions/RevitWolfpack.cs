using Autodesk.Revit.DB;
using Direwolf.Contracts;
using Direwolf.Definitions;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct RevitWolfpack : IWolfpack
    {
        public RevitWolfpack(Document doc,
            IHowler howler,
            string fileOrigin = "",
            string testName = "",
            double timeTaken = 0,
            bool wasCompleted = false)
        {
            CreatedAt = DateTime.UtcNow;
            FileOrigin = fileOrigin;
            GUID = new Guid().ToString();
            ResultCount = howler.Den.Count;
            TestName = testName;
            TimeTaken = timeTaken;
            WasCompleted = wasCompleted;
            Document = new DocumentIntrospection(doc);
            ProjectInformation = new ProjectInformationIntrospection(doc);
            Site = new ProjectSiteIntrospection(doc);
            Units = new ProjectUnitsIntrospection(doc);
            foreach (var w in doc.GetWarnings())
            {
                Warnings.Add(new WarningIntrospection(w));
            }
            Results = howler.ToString();

            BasicFileInfo b = BasicFileInfo.Extract(doc.PathName);
            DocumentSessionId = b.GetDocumentVersion().VersionGUID.ToString();
            DocumentCreationId = doc.CreationGUID.ToString();
            ChangedElements = [.. doc.GetChangedElements(b.GetDocumentVersion().VersionGUID).GetCreatedElementIds().Select(x => x.Value)];
        }
        public DateTime CreatedAt { get; init; }
        public string FileOrigin { get; init; }
        public string GUID { get; init; }
        public string DocumentSessionId { get; init; }
        public string DocumentCreationId { get; init; }
        public List<long> ChangedElements { get; init; }
        public int ResultCount { get; init; }
        public string? Results { get; init; }
        public string TestName { get; init; }
        public double TimeTaken { get; init; }
        public bool WasCompleted { get; init; }
        public DocumentIntrospection Document { get; init; }
        public ProjectInformationIntrospection ProjectInformation { get; init; }
        public ProjectSiteIntrospection Site { get; init; }
        public ProjectUnitsIntrospection Units { get; init; }
        public List<WarningIntrospection> Warnings { get; init; } = [];

        public static string ToSql()
        {
            return """
                INSERT INTO "Wolfpack"(
                "projectInformation",
                "documentInformation",
                "siteInformation",
                "unitsInformation",
                "warnings",
                "documentSessionId",
                "documentCreationId",
                "changedElements",
                "fileOrigin",
                "wasCompleted",
                "timeTaken",
                "createdAt",
                "guid",
                "resultCount",
                "testName",
                "results",
                "elementInfo"
                ) VALUES (
                @projectInformation,
                @documentInformation,
                @siteInformation,
                @warnings,
                @documentSessionId,
                @changedElements,
                @fileOrigin,
                @wasCompleted,
                @timeTaken,
                @createdAt,
                @guid,
                @resultCount,
                @testName,
                @results,
                @elementInfo)
                ON CONFLICT (guid) DO NOTHING;
                """;
        }
    }
}
