using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct _ParameterInformation()
    {
        public required string parameterGuid { get; init; }
        public required string documentOwner { get; init; }
        public StorageType storageType { get;init; }
        public bool hasValue { get; init; }
        public double parameterIdValue { get; init; }
        public bool isReadOnly { get; init; }
        public bool isShared { get; init; }
        public bool isUserModifiable { get; init; }
    }
}
