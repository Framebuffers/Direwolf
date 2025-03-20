using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions
{

    public readonly record struct ElementInformation()
    {
        public required double idValue { get; init; }
        public required string uniqueElementId { get; init; }
        public required string elementVersionId { get; init; }
        public string? familyName { get; init; }
        public string? category { get; init; }
        public string? builtInCategory { get; init; }
        public string? workset { get; init; }
        public string[]? views { get; init; }
        public string? designOption { get; init; }
        public string? documentOwner { get; init; }
        public string? ownerViewId { get; init; }
        public string? worksetId { get; init; }
        public string? levelId { get; init; }
        public string? createdPhaseId { get; init; }
        public string? demolishedPhaseId { get; init; }
        public string? groupId { get; init; }
        public string? workshareId { get; init; }
        public bool? isGrouped { get; init; }
        public bool? isModifiable { get; init; }
        public bool? isViewSpecific { get; init; }
        public bool? isBuiltInCategory { get; init; }
        public bool? isAnnotative { get; init; }
        public bool? isModel { get; init; }
        public bool? isPinned { get; init; }
        public bool? isWorkshared { get; init; }
        public Dictionary<string, string>? Parameters { get; init; }
    }
    
    public readonly record struct ParameterInformation()
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

    public readonly record struct ModelIntrospectionInformation
    {
        public double elementCountTotal { get; init; }
        public Dictionary<string, List<string>> familiyElementCount { get; init; }
        public string documentVersion { get; init; }
        public long fileSize { get;init; }

    }
}
