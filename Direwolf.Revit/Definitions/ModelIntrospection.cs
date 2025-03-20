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
        public int viewsInsideDocument { get; init; }
        public int notInSheets { get; init; }
        public int annotativeElements { get; init; }
        public Dictionary<string, string>? externalRefs { get; init; }
        public int modelGroups { get; init; }
        public int detailGroups { get; init; }
        public int designOptions { get; init; }
        public int levels { get; init; }
        public int grids { get; init; }
        public int warns { get; init; }
        public int unenclosedRoom { get; init; }
        public int viewports { get; init; }
        public int unconnectedDucts { get; init; }
        public int unconnectedPipes { get; init; }
        public int unconnectedElectrical { get; init; }
        public int nonNativeStyles { get; init; }
        public int isFlipped { get; init; }
        public Dictionary<string, int> worksetElementCount { get; init; }
    }
}
