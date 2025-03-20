namespace Direwolf.Revit.Definitions
{
    public readonly record struct ModelIntrospection
    {
        public double elementCountTotal { get; init; }
        public Dictionary<string, List<string>> familiyElementCount { get; init; }
        public string documentVersion { get; init; }
        public long fileSize { get; init; }
        public List<string> warnings { get; init; }
        public Dictionary<string, double> elementCount { get; init; }
        public Dictionary<ElementInformation, List<ParameterInformation?>>? viewsInsideDocument { get; init; }
        public Dictionary<ElementInformation, List<ParameterInformation?>>? notInSheets { get; init; }
        public Dictionary<ElementInformation, List<ParameterInformation?>>? annotativeElements { get; init; }
        public Dictionary<string, string>? externalRefs { get; init; }
        public Dictionary<ElementInformation, List<ParameterInformation?>>? modelGroups { get; init; }
        public Dictionary<ElementInformation, List<ParameterInformation?>>? detailGroups { get; init; }
        public Dictionary<ElementInformation, List<ParameterInformation?>>? designOptions { get; init; }
        public Dictionary<ElementInformation, List<ParameterInformation?>>? levels { get; init; }
        public Dictionary<ElementInformation, List<ParameterInformation?>>? grids { get; init; }
        public Dictionary<ElementInformation, List<ParameterInformation?>>? warns { get; init; }
        public Dictionary<ElementInformation, List<ParameterInformation?>>? unenclosedRoom { get; init; }
        public Dictionary<ElementInformation, List<ParameterInformation?>>? viewports { get; init; }
        public Dictionary<ElementInformation, List<ParameterInformation?>>? unconnectedDucts { get; init; }
        public Dictionary<ElementInformation, List<ParameterInformation?>>? unconnectedPipes { get; init; }
        public Dictionary<ElementInformation, List<ParameterInformation?>>? unconnectedElectrical { get; init; }
        public Dictionary<ElementInformation, List<ParameterInformation?>>? nonNativeStyles { get; init; }
        public Dictionary<ElementInformation, List<ParameterInformation?>>? isFlipped { get; init; }
        public Dictionary<string, int> worksetElementCount { get; init; }
    }
}
