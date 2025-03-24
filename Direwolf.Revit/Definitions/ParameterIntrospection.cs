namespace Direwolf.Revit.Definitions
{
    public readonly record struct ParameterIntrospection
    {
        public double id { get; init; }
        public string? name { get; init; }
        public string? value { get; init; }
        public string? storageType { get; init; }
        public string? unitTypeId { get; init; }
        public string? parentElement { get; init; }
        public bool? hasValue { get; init; }
        public bool? userModifiable { get; init; }
        public bool? isShared { get; init; }
        public string? sharedParameterGuid { get; init; }
        public string? dataType { get; init; }
        public string? groupTypeId { get; init; }
    }
 }
