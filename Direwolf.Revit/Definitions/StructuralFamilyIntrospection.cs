namespace Direwolf.Revit.Definitions
{
    public readonly record struct StructuralFamilyIntrospection
    {
        public string? structuralCodeName { get; init; }
        public string? structuralFamilyNameKey { get; init; }
        public string? structuralMaterialType { get; init; }
        public string? structuralSectionShape { get; init; }
    }
}

