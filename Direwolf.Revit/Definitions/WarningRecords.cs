using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct WarningRecords
    {
        public Guid RecordUniqueId { get; init; } = Guid.NewGuid();
        public string DocumentUniqueId { get; init; }
        public string EpisodeId { get; init; }
        public int SaveCountIndex { get; init; }
        public DateTime CreatedAt { get; init; }
        public int SeverityLevelNoneCount { get; init; }
        public int SeverityLevelWarningCount { get; init; }
        public int SeverityLevelErrorCount { get; init; }
        public int SeverityLevelDocumentCorruptionCount { get; init; }
        public string JournalFileName { get; init; }
        public List<WarningRecord> Warnings { get; init; } = [];

        public WarningRecords(Document doc)
        {
            DocumentUniqueId = doc.CreationGUID.ToString();
            EpisodeId = Document.GetDocumentVersion(doc).VersionGUID.ToString();
            SaveCountIndex = Document.GetDocumentVersion(doc).NumberOfSaves;
            CreatedAt = DateTime.UtcNow;
            JournalFileName = doc.Application.RecordingJournalFilename;
            foreach (var warning in doc.GetWarnings())
            {
                Warnings.Add(new WarningRecord()
                {
                    Message = warning.GetDescriptionText(),
                    Severity = warning.GetSeverity().ToString(),
                    FailingElements = [.. warning.GetFailingElements().Select(x => x.Value)]
                });

                switch (warning.GetSeverity())
                {
                    case FailureSeverity.None:
                        SeverityLevelNoneCount++;
                        break;
                    case FailureSeverity.Warning:
                        SeverityLevelWarningCount++;
                        break;
                    case FailureSeverity.Error:
                        SeverityLevelErrorCount++;
                        break;
                    case FailureSeverity.DocumentCorruption:
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
