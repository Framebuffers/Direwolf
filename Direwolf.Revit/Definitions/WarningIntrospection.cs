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
    }
}
