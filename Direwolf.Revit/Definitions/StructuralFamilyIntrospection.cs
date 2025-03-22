using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Structure.StructuralSections;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct StructuralFamilyIntrospection(Family family)
    {
        public string structuralCodeName
        {
            get
            {
                try
                {
                    return family.StructuralCodeName;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        public string structuralFamilyNameKey
        {
            get
            {
                try
                {
                    return family.StructuralCodeName;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        public string structuralMaterialType
        {
            get
            {
                try
                {
                    return family.StructuralMaterialType.ToString();
                }
                catch
                {
                    return StructuralMaterialType.Undefined.ToString();
                }
            }
        }
        public string structuralSectionShape
        {
            get
            {
                try
                {
                    return family.StructuralSectionShape.ToString();
                }
                catch
                {
                    return StructuralSectionShape.Invalid.ToString();
                }
            }
        }

    }
}

