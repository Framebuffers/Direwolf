using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct FamilyInstanceIntrospection(FamilyInstance f)
    {
        public double? id { get; init; }
        public string? uniqueId { get; init; }
        public string? versionId { get; init; }
        public bool? canFlipFacing { get; init; }
        public bool? canFlipHand { get; init; }
        public bool? canFlipWorkplane { get; init; }
        public bool? canRotate { get; init; }
        public bool? canSplit { get; init; }
        public bool? facingFlipped { get; init; }
        public double? facingOrientationX { get; init; }
        public double? facingOrientationY { get; init; }
        public double? facingOrientationZ { get; init; }
        public bool? handFlipped { get; init; }
        public double? handOrientationX { get; init; }
        public double? handOrientationY { get; init; }
        public double? handOrientationZ { get; init; }
        public bool? hasSpatialElementCalculationPoint { get; init; }
        public bool? hasSpatialElementFromToCalculationPoints { get; init; }
        public string? hostId { get; init; }
        public bool? invisible { get; init; }
        public bool? isSlantedColumn { get; init; }
        public bool? isPinned { get; init; }
        public string? roomId { get; init; }
        public string? spaceId { get; init; }
        public string? superComponentId { get; init; }
        public string? symbolId { get; init; }
    }
}

