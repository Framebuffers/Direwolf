namespace Direwolf.Revit.Definitions
{
    public readonly record struct ModelDatabase(
        Dictionary<string, Dictionary<FamilyIntrospection, HashSet<FamilyInstanceIntrospection>>> ElementsByCategory,
        DocumentIntrospection DocumentInfo,
        ProjectInformationIntrospection ProjectInfo,
        UnitIntrospection Units)
    {
    }
}
