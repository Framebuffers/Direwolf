using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct WarningCollectionRecord
    {
        public Guid WarningCollectionUniqueId { get; init; } = Guid.NewGuid();
        public Guid ParentDocumentEpisodeUniqueId { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public int SeverityLevelNoneCount { get; init; } = 0;
        public int SeverityLevelWarningCount { get; init; } = 0;
        public int SeverityLevelErrorCount { get; init; } = 0;
        public int SeverityLevelDocumentCorruptionCount { get; init; } = 0;
        public string JournalFileName { get; init; } = string.Empty;
        public List<WarningRecord> Warnings { get; init; } = [];

        public WarningCollectionRecord(Document doc, Guid parent)
        {
            ParentDocumentEpisodeUniqueId = parent;
            JournalFileName = doc.Application.RecordingJournalFilename ?? string.Empty;

            if (doc.GetWarnings() is not null)
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
                        SeverityLevelDocumentCorruptionCount++;
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
