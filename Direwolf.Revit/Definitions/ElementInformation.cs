namespace Direwolf.Revit.Definitions
{

    public readonly record struct ElementInformation()
    {
        public required double ElementIdValue { get; init; }
        public required string ElementUniqueId { get; init; }
        public required string ElementVersionId { get; init; }
        public string? FamilyName { get; init; }
        public string? Category { get; init; }
        public string? BuiltInCategory { get; init; }
        public string? Workset { get; init; }
        public string[]? Views { get; init; }
        public string? DesignOption { get; init; }
        public string? DocumentOwner { get; init; }
        public string? OwnerViewId { get; init; }
        public string? WorksetId { get; init; }
        public string? LevelId { get; init; }
        public string? CreatedPhaseId { get; init; }
        public string? DemolishedPhaseId { get; init; }
        public string? GroupId { get; init; }
        public string? WorkshareId { get; init; }
        public bool? IsGrouped { get; init; }
        public bool? IsModifiable { get; init; }
        public bool? IsViewSpecific { get; init; }
        public bool? IsBuiltInCategory { get; init; }
        public bool? IsAnnotative { get; init; }
        public bool? IsModel { get; init; }
        public bool? IsPinned { get; init; }
        public bool? IsWorkshared { get; init; }
        public Dictionary<string, string>? Parameters { get; init; }
    }



}
