namespace Direwolf.Revit.Definitions
{
    public readonly record struct _ModelIntrospectionInformation
    {
        public double elementCountTotal { get; init; }
        public Dictionary<string, List<string>> familiyElementCount { get; init; }
        public string documentVersion { get; init; }
        public long fileSize { get;init; }
    }
}
