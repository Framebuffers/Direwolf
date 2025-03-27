using Autodesk.Revit.DB;
using Direwolf.Revit.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct WarningRecord
    {
        public Guid RecordUniqueId { get; init; }
        public string Message { get; init; }
        public string Severity { get; init; }
        public List<long> FailingElements { get; init; }

        public WarningRecord(FailureMessage f)
        {
            RecordUniqueId = Guid.NewGuid();
            Message = f.GetDescriptionText();
            Severity = f.GetSeverity().ToString();
            FailingElements = [.. f.GetFailingElements().Select(x => x.Value)];
        }

        //public static string AsSql()
        //{
        //    return """
        //        INSERT INTO "DocumentWarning" (
        //        "documentId",
        //        "document",
        //        "createdAt",
        //        "severity",
        //        "message",
        //        "failingElements"
        //        ) VALUES (
        //        @documentId,
        //        @document,
        //        @createdAt,
        //        @severity,
        //        @message);
        //        """;
        //}
    }
}
