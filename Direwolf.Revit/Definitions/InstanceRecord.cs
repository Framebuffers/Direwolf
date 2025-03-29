using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct InstanceRecord()
    {
        public Guid InstanceRecordUniqueId { get; init; }
        public required string ParentElementUniqueId { get; init; }
        public double? InstanceValueId { get; init; }
        public string? InstanceUniqueId { get; init; }
        public string? InstanceFamilyName { get; init; }
        public string? InstanceBuiltInCategory { get; init; }
        public string? InstanceWorkset { get; init; }
        public string[]? InstanceViews { get; init; }
        public bool? InstanceIsGrouped { get; init; }
        public bool? InstanceIsModifiable { get; init; }
        public bool? InstanceIsViewSpecific { get; init; }
        public bool? InstanceIsBuiltInCategory { get; init; }
        public bool? InstanceIsAnnotative { get; init; }
        public bool? InstanceIsModel { get; init; }
        public bool? InstanceIsPinned { get; init; }
        public bool? InstanceIsWorkshared { get; init; }
        public double? InstanceOwnerViewValueId { get; init; }
        public string? InstanceDesignOptionUniqueId { get; init; }
        public string? InstanceWorksetUniqueId{ get; init; }
        public string? InstanceLevelUniqueUniqueId { get; init; }
        public string? InstanceCreatedPhaseUniqueId { get; init; }
        public string? InstanceDemolishedPhaseUniqueId { get; init; }
        public string? InstanceGroupUniqueId { get; init; }
        public Guid InstanceParametersUniqueId { get; init; }
    }
}
