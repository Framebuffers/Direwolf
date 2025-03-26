using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct ParameterIntrospection
    {
        public DateTime CreatedAt { get; init; }
        public string ParameterGuid { get; init; }
        public string DocumentOwner { get; init; }
        public string ParentElementInfo { get; init; }
        public StorageType StorageType { get; init; }
        public string Name { get; init; }
        public string Value { get; init; }
        public bool IsReadOnly { get; init; }
        public string TypeId { get; init; }
        public string DataType { get; init; }
        public string GroupTypeId { get; init; }
        public bool HasValue { get; init; }
        public bool IsShared { get; init; }
        public bool IsUserModifiable { get; init; }
        public string SharedParameterGuid { get; init; }

    }
}
