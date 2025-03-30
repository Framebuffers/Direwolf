using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct WarningIntrospection
    {
        public WarningIntrospection(FailureMessage f)
        {
            Message = f.GetDescriptionText();
            Severity = f.GetSeverity().ToString();
            FailingElements = f.GetFailingElements().Select(x => x.Value).ToList();
            CreatedAt = DateTime.UtcNow;
        }
        public DateTime CreatedAt { get; init; }
        public string Message { get; init; }
        public string Severity { get; init; }
        public List<long> FailingElements { get; init; }

        public static string AsSql()
        {
            return """
                INSERT INTO "DocumentWarning" (
                "documentId",
                "document",
                "createdAt",
                "severity",
                "message",
                "failingElements"
                ) VALUES (
                @documentId,
                @document,
                @createdAt,
                @severity,
                @message);
                """;
        }
    }
}
