using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct ParameterInformation()
    {
        public required string parameterGuid { get; init; }
        public required string documentOwner { get; init; }
        public StorageType storageType { get; init; }
        public object? value { get; init; }
        public bool hasValue { get; init; }
        public double parameterIdValue { get; init; }
    }
}
