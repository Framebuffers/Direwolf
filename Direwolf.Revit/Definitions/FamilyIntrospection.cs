using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct FamilyIntrospection(Family family)
    {
        public static FamilyIntrospection CreateFromSymbol(FamilyInstance fi)
        {
            return new FamilyIntrospection(fi.Symbol.Family);
        }
        public string name
        {
            get
            {
                try
                {
                    return family.Category.Name;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        public double id
        {
            get
            {
                try
                {
                    return family.Id.Value;
                }
                catch
                {
                    return -1;
                }
            }
        }
        public string uniqueId
        {
            get
            {
                try
                {
                    return family.UniqueId;
                }
                catch
                {
                    return Guid.Empty.ToString();
                }
            }
        }
        public string builtInCategory
        {
            get
            {
                try
                {
                    return family.Category.BuiltInCategory.ToString();
                }
                catch
                {
                    return BuiltInCategory.INVALID.ToString();
                }
            }
        }
        public string familyCategory
        {
            get
            {
                try
                {
                    return family.FamilyCategory.Name;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        public double familyCategoryId
        {
            get
            {
                try
                {
                    return family.FamilyCategoryId.Value;
                }
                catch
                {
                    return -1;
                }
            }
        }
        public string familyPlacementType
        {
            get
            {
                try
                {
                    return family.FamilyPlacementType.ToString();
                }
                catch
                {
                    return FamilyPlacementType.Invalid.ToString();
                }
            }
        }
        public bool isConceptualMassFamily
        {
            get
            {
                try
                {
                    return family.IsConceptualMassFamily;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool isCurtainWallPanelFamily
        {
            get
            {
                try
                {
                    return family.IsCurtainPanelFamily;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool isEditable
        {
            get
            {
                try
                {
                    return family.IsEditable;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool isInPlace
        {
            get
            {
                try
                {
                    return family.IsInPlace;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool isOwnerFamily
        {
            get
            {
                try
                {
                    return family.IsOwnerFamily;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool isParametric
        {
            get
            {
                try
                {
                    return family.IsParametric;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool isUserCreated
        {
            get
            {
                try
                {
                    return family.IsUserCreated;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}

