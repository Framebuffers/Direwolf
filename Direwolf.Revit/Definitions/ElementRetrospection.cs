using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct ElementRetrospection
    {
        public double? Id { get; init; }
        public string? UniqueId { get; init; }
        public int? BuiltInCategory { get; init; }
        public double? AssemblyInstanceId { get; init; }
        public double? CreatedPhaseId { get; init; }
        public double? DemolishedPhaseId { get; init; }
        public bool? HasDesignOption { get; init; }
        public double? GroupId { get; init; }
        public double? LevelId { get; init; }
        public string? Location { get; init; }
        public string? Name { get; init; }
        public double? OwnerViewId { get; init; }
        public bool? IsPinned { get; init; }
        public bool? IsViewSpecific { get; init; }
        public double? WorksetId { get; init; }

        public ElementRetrospection(Element element)
        {
            Id = element.Id.Value;
            UniqueId = element.UniqueId;
            BuiltInCategory = (int)element.Category?.BuiltInCategory;
            AssemblyInstanceId = element.AssemblyInstanceId?.Value ?? -1;
            CreatedPhaseId = element.CreatedPhaseId?.Value ?? -1;
            DemolishedPhaseId = element.DemolishedPhaseId?.Value ?? -1;
            HasDesignOption = element.DesignOption is not null;
            GroupId = element.GroupId?.Value != -1 ? element.GroupId.Value : -1;
            LevelId = element.LevelId?.Value ?? -1;
            Location = element.Location?.ToString() ?? string.Empty;
            Name = element.Name ?? string.Empty;
            OwnerViewId = element.OwnerViewId?.Value ?? -1;
            IsPinned = element.Pinned;
            IsViewSpecific = element.ViewSpecific;
            WorksetId = element.WorksetId?.IntegerValue ?? -1;
        }

        public ElementRetrospection() { }

    }

}





