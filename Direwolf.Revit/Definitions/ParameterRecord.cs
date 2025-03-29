using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct ParameterRecord
    {
        public Guid ParameterRecordUniqueId { get; init; }
        public Guid ParentInstanceFK { get; init; }
        public DateTime ParameterCreatedAt { get; init; }
        public bool? HasValue { get; init; }
        public bool? IsReadOnly { get; init; }
        public bool? IsShared { get; init; }
        public bool? IsUserModifiable { get; init; }
        public string? StorageType { get; init; }
        public string? Name { get; init; }
        public string? Value { get; init; }
        public string? ForgeTypeId { get; init; }
        public string? ForgeDataType { get; init; }
        public string? ForgeGroupTypeId { get; init; }
        public string? SharedParameterGuid { get; init; }
    }
}
