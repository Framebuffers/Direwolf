using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct FamilyIntrospection(Family family)
    {
        public double id { get; init; }
        public string name { get; init; }
        public string uniqueId { get; init; }
        public string builtInCategory { get; init; }
        public string familyCategory { get; init; }
        public double familyCategoryId { get; init; }
        public string familyPlacementType { get; init; }
        public bool isConceptualMassFamily { get; init; }
        public bool isCurtainWallPanelFamily { get; init; }
        public bool isEditable { get; init; }
        public bool isInPlace { get; init; }
        public bool isOwnerFamily { get; init; }
        public bool isParametric { get; init; }
        public bool isUserCreated { get; init; }
    }
}
