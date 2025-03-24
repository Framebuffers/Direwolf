using Autodesk.Revit.DB;
using Direwolf.Revit.Definitions;

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
        //public static FamilyIntrospection CreateFromSymbol(FamilyInstance fi)
        //{
        //    return new FamilyIntrospection(fi.Symbol.Family);
        //}public HealthIndicatorIntrospection h = new()
        {
            viewsInsideDocument = viewsInsideDocument.Count,
            notInSheets = 0,
            annotativeElements = 0,
            externalRefs = 0,
            modelGroups = 0,
            detailGroups = 0,
            designOptions = 0,
            levels = 0,
            grids = 0,
            List = new List<string>(),
            unenclosedRoom = 0,
            viewports = 0,
            unconnectedDucts = 0,
            unconnectedPipes = 0,
            unconnectedElectrical = 0,
            nonNativeStyles = 0,
            isFlipped = 0,
            worksetElementCount = new Dictionary<string, int>(),

        };

        //public string name
        //{
        //    get
        //    {
        //        try
        //        {
        //            return family.Category.Name;
        //        }
        //        catch
        //        {
        //            return string.Empty;
        //        }
        //    }
        //}
        //public double id
        //{
        //    get
        //    {
        //        try
        //        {
        //            return family.Id.Value;
        //        }
        //        catch
        //        {
        //            return -1;
        //        }
        //    }
        //}
        //public string uniqueId
        //{
        //    get
        //    {
        //        try
        //        {
        //            return family.UniqueId;
        //        }
        //        catch
        //        {
        //            return Guid.Empty.ToString();
        //        }
        //    }
        //}
        //public string builtInCategory
        //{
        //    get
        //    {
        //        try
        //        {
        //            return family.Category.BuiltInCategory.ToString();
        //        }
        //        catch
        //        {
        //            return BuiltInCategory.INVALID.ToString();
        //        }
        //    }
        //}
        //public string familyCategory
        //{
        //    get
        //    {
        //        try
        //        {
        //            return family.FamilyCategory.Name;
        //        }
        //        catch
        //        {
        //            return string.Empty;
        //        }
        //    }
        //}
        //public double familyCategoryId
        //{
        //    get
        //    {
        //        try
        //        {
        //            return family.FamilyCategoryId.Value;
        //        }
        //        catch
        //        {
        //            return -1;
        //        }
        //    }
        //}
        //public string familyPlacementType
        //{
        //    get
        //    {
        //        try
        //        {
        //            return family.FamilyPlacementType.ToString();
        //        }
        //        catch
        //        {
        //            return FamilyPlacementType.Invalid.ToString();
        //        }
        //    }
        //}
        //public bool isConceptualMassFamily
        //{
        //    get
        //    {
        //        try
        //        {
        //            return family.IsConceptualMassFamily;
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }
        //}
        //public bool isCurtainWallPanelFamily
        //{
        //    get
        //    {
        //        try
        //        {
        //            return family.IsCurtainPanelFamily;
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }
        //}
        //public bool isEditable
        //{
        //    get
        //    {
        //        try
        //        {
        //            return family.IsEditable;
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }
        //}
        //public bool isInPlace
        //{
        //    get
        //    {
        //        try
        //        {
        //            return family.IsInPlace;
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }
        //}
        //public bool isOwnerFamily
        //{
        //    get
        //    {
        //        try
        //        {
        //            return family.IsOwnerFamily;
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }
        //}
        //public bool isParametric
        //{
        //    get
        //    {
        //        try
        //        {
        //            return family.IsParametric;
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }
        //}
        //public bool isUserCreated
        //{
        //    get
        //    {
        //        try
        //        {
        //            return family.IsUserCreated;
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }
        //}
    }
}
public HealthIndicatorIntrospection h = new()
{
    viewsInsideDocument = viewsInsideDocument.Count,
    notInSheets = 0,
    annotativeElements = 0,
    externalRefs = 0,
    modelGroups = 0,
    detailGroups = 0,
    designOptions = 0,
    levels = 0,
    grids = 0,
    List = new List<string>(),
    unenclosedRoom = 0,
    viewports = 0,
    unconnectedDucts = 0,
    unconnectedPipes = 0,
    unconnectedElectrical = 0,
    nonNativeStyles = 0,
    isFlipped = 0,
    worksetElementCount = new Dictionary<string, int>(),

};

