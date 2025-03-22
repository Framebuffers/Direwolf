using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct FamilyInstanceIntrospection(FamilyInstance f)
    {
        public double id
        {
            get
            {
                try
                {
                    return f.Id.Value;
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
                    return f.UniqueId;
                }
                catch
                {
                    return Guid.Empty.ToString();
                }
            }
        }
        public Guid versionId
        {
            get
            {
                try
                {
                    return f.VersionGuid;
                }
                catch
                {
                    return Guid.Empty;
                }
            }
        }
        public bool canFlipFacing
        {
            get
            {
                try
                {
                    return f.CanFlipFacing;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool canFlipHand
        {
            get
            {
                try
                {
                    return f.CanFlipHand;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool canFlipWorkplane
        {
            get
            {
                try
                {
                    return f.CanFlipWorkPlane;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool canRotate
        {
            get
            {
                try
                {
                    return f.CanRotate;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool canSplit
        {
            get
            {
                try
                {
                    return f.CanSplit;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool facingFlipped
        {
            get
            {
                try
                {
                    return f.FacingFlipped;
                }
                catch
                {
                    return false;
                }
            }
        }
        public double facingOrientationX
        {
            get
            {
                try
                {
                    return f.FacingOrientation.X;
                }
                catch
                {
                    return -1;
                }
            }
        }
        public double facingOrientationY
        {
            get
            {
                try
                {
                    return f.FacingOrientation.Y;
                }
                catch
                {
                    return -1;
                }
            }
        }
        public double facingOrientationZ
        {
            get
            {
                try
                {
                    return f.FacingOrientation.Z;
                }
                catch
                {
                    return -1;
                }
            }
        }
        public bool handFlipped
        {
            get
            {
                try
                {
                    return f.HandFlipped;
                }
                catch
                {
                    return false;
                }
            }
        }
        public double handOrientationX
        {
            get
            {
                try
                {
                    return f.HandOrientation.X;
                }
                catch
                {
                    return -1;
                }
            }
        }
        public double handOrientationY
        {
            get
            {
                try
                {
                    return f.HandOrientation.Y;
                }
                catch
                {
                    return -1;
                }
            }
        }
        public double handOrientationZ
        {
            get
            {
                try
                {
                    return f.HandOrientation.Z;
                }
                catch
                {
                    return -1;
                }
            }
        }
        public bool hasSpatialElementCalculationPoint
        {
            get
            {
                try
                {
                    return f.HasSpatialElementCalculationPoint;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool hasSpatialElementFromToCalculationPoints
        {
            get
            {
                try
                {
                    return f.HasSpatialElementFromToCalculationPoints;
                }
                catch
                {
                    return false;
                }
            }
        }
        public string hostId
        {
            get
            {
                try
                {
                    return f.Host.UniqueId;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        public bool invisible
        {
            get
            {
                try
                {
                    return f.Invisible;
                }
                catch
                {
                    /*
                     * Even if, logically, an invalid element is invisible; to make the whole logic consistent, it defaults to false.
                     */
                    return false;
                }
            }
        }
        public bool isSlantedColumn
        {
            get
            {
                try
                {
                    return f.IsSlantedColumn;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool isPinned
        {
            get
            {
                try
                {
                    return f.Pinned;
                }
                catch
                {
                    return false;
                }
            }
        }
        public string roomId
        {
            get
            {
                try
                {
                    return f.Room.UniqueId;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        public string spaceId
        {
            get
            {
                try
                {
                    return f.Space.UniqueId;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        public string superComponentId
        {
            get
            {
                try
                {
                    return f.SuperComponent.UniqueId;
                }
                catch
                {
                    return Guid.Empty.ToString();
                }
            }
        }
        public string symbolId
        {
            get
            {
                try
                {
                    return f.Symbol.UniqueId;
                }
                catch
                {
                    return Guid.Empty.ToString();
                }
            }
        }

    }

    public readonly record struct StructuralFamilyInstanceIntrospection(FamilyInstance f)
    {
        public double structuralMaterialId
        {
            get
            {
                try
                {
                    return f.StructuralMaterialId.Value;
                }
                catch
                {
                    return -1;
                }
            }
        }
        public string structuralMaterialType
        {
            get
            {
                try
                {
                    return f.StructuralMaterialType.ToString();
                }
                catch
                {
                    return StructuralMaterialType.Undefined.ToString();
                }
            }
        }
        public string structuralType
        {
            get
            {
                try
                {
                    return f.StructuralType.ToString();
                }
                catch
                {
                    return StructuralType.UnknownFraming.ToString();
                }
            }
        }
        public string structuralUsage
        {
            get
            {
                try
                {
                    return f.StructuralUsage.ToString();
                }
                catch
                {
                    return StructuralInstanceUsage.Undefined.ToString();
                }
            }
        }

    }
}
