using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct FamilyRecord()
    {
        public Guid RecordUniqueId { get; init; } = Guid.NewGuid();
        public Guid DocumentUniqueId { get; init; }

        // Conditions
        public bool? ArePhasesModifiable { get; init; }
        public bool? CanBeLocked { get; init; }
        public bool? CanFlipFacing { get; init; }
        public bool? CanFlipHand { get; init; }
        public bool? CanFlipWorkPlane { get; init; }
        public bool? CanHaveAnalyticalModel { get; init; }
        public bool? CanHaveTypeAssigned { get; init; }
        public bool? CanRotate { get; init; }
        public bool? CanSplit { get; init; }
        public bool? FacingFlipped { get; init; }
        public bool? HandFlipped { get; init; }
        public bool? HasModifiedGeometry { get; init; }
        public bool? HasPhases { get; init; }
        public bool? HasSpatialElementCalculationPoint { get; init; }
        public bool? HasSpatialElementFromToCalculationPoints { get; init; }
        public bool? HasSweptProfile { get; init; }
        public bool? IsExternalFileReference { get; init; }
        public bool? IsHidden { get; init; }
        public bool? IsMirrored { get; init; }
        public bool? IsMonitoringLinkElement { get; init; }
        public bool? IsMonitoringLocalElement { get; init; }
        public bool? IsPhaseCreateValid { get; init; }
        public bool? IsPhaseDemolishedValid { get; init; }
        public bool? IsPinned { get; init; }
        public bool? IsSlantedColumn { get; init; }
        public bool? IsViewSpecific { get; init; }
        public bool? RefersToExternalResourceReferences { get; init; }
        public bool? TransformHasReflection { get; init; }
        public bool? TransformIsConformal { get; init; }
        public bool? TransformOrigin { get; init; }
        public bool? TransformScale { get; init; }
        public bool? IsVisible { get; init; }

        // Transient References
        public double? AnalyticalModelValueId { get; init; }
        public double? CreatedPhaseValueId { get; init; }
        public double? DemolishedPhaseValueId { get; init; }
        public double? HostFaceValueId { get; init; }
        public double? HostParameterValue { get; init; }
        public double? LevelValueId { get; init; }
        public double[]? CopingsValueId { get; init; }
        public double[]? DependentElementsValueId { get; init; }
        public double[]? MaterialsValueId { get; init; }
        public double[]? MonitoredLinksValueId { get; init; }
        public double[]? ReferencesValueId { get; init; }
        public double[]? SubComponentsValueId { get; init; }
        public double? WorksetValueId { get; init; }

        // Stable references
        public string? DesignOptionElementUniqueId { get; init; }
        public string? FamilySymbolUniqueId { get; init; }
        public string? FromRoomUniqueId { get; init; }
        public string? HostUniqueId { get; init; }
        public string? PhaseUniqueId { get; init; }
        public string? RoomUniqueId { get; init; }
        public string? SpaceUniqueId { get; init; }
        public string? SuperComponentUniqueId { get; init; }
        public string? TypeId { get; init; }

        // Properties
        public string? BuiltInCategoryName { get; init; }
        public string? FamilyName { get; init; }
        public string? CategoryType { get; init; }
        public double? FamilyTypesCount { get; init; }
        public double? FamilyInstancesCount { get; init; }
        public string[]? FamilyInstancesUniqueId { get; init; }
        public List<Vector3>? SpatialElementCalculationPoints { get; init; }
        public string? PhaseStatus { get; init; }
        public IDictionary<string, string>? ExternalResourceReferences { get; init; }
        public string[]? EntitySchemasUniqueId { get; init; }

        // Geometry
        public Vector3? HandOrientation { get; init; }
        public Vector3? TransformBasis { get; init; }
        public double? TransformDeterminant { get; init; }
        public Vector3? TransformIdentityBasis { get; init; }
        public string? FamilyPointPlacementLocation { get; init; }
        public string? FamilyPointPlacementName { get; init; }
        public string? FamilyPointReference { get; init; }
        public Vector3? FacingOrientation { get; init; }

        // Structural
        public double? StructuralMaterialId { get; init; }
        public string? StructuralMaterialType { get; init; }
        public string? StructuralType { get; init; }
        public string? StructuralInstanceUsage { get; init; }
    }
}
