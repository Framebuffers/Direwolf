using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct ElementIntrospection(Element Element)
    {
        public double? id { get; init; }

        public bool? hasDesignOption { get; init; }
        public bool? isPinned { get; init; }
        public bool? isViewSpecific { get; init; }
        public bool? isModifiable { get; init; }
        public bool? isGrouped { get; init; }
        public bool? isBuiltInCategory { get; init; }
        public bool? isAnnotative { get; init; }
        public bool? isModel { get; init; }
        public bool? isWorkshared { get; init; }

        public double? assemblyInstanceId { get; init; }
        public double? createdPhaseId { get; init; }
        public double? demolishedPhaseId { get; init; }
        public double? groupId { get; init; }
        public double? levelId { get; init; }
        public double? ownerViewId { get; init; }

        public int? worksetId { get; init; }

        public string? uniqueId { get; init; }
        public string? elementVersionId { get; init; }
        public string? documentOwnerId { get; init; }
        public string? builtInCategory { get; init; }
        public string? location { get; init; }
        public string? name { get; init; }
        public string? workshareId { get; init; }

        public IList<ParameterIntrospection>? parameters { get; init; }

    }

}
