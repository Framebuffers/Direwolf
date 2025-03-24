namespace Direwolf.Revit.Definitions
{
    public readonly record struct HealthIndicatorIntrospection
    {
        public double? viewsInsideDocument { get; init; }
        public double? notInSheets { get; init; }
        public double? annotativeElements { get; init; }
        public double? externalRefs { get; init; }
        public double? modelGroups { get; init; }
        public double? detailGroups { get; init; }
        public double? designOptions { get; init; }
        public double? levels { get; init; }
        public double? grids { get; init; }
        public List<string>? warnings { get; init; }
        public double? unenclosedRoom { get; init; }
        public double? viewports { get; init; }
        public double? unconnectedDucts { get; init; }
        public double? unconnectedPipes { get; init; }
        public double? unconnectedElectrical { get; init; }
        public double? nonNativeStyles { get; init; }
        public double? isFlipped { get; init; }
        public Dictionary<string, int> worksetElementCount { get; init; }
    }
}
