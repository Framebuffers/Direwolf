using Autodesk.Revit.DB;
using Direwolf.Revit.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct CategoryIntrospection(Category c)
    {
        public double id
        {
            get
            {
                try
                {
                    return c.Id.Value;
                }
                catch
                {
                    return -1;
                }
            }
        }
        public string name
        {
            get
            {
                try
                {
                    return c.Name;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        public string builtInCategory
        {
            get
            {
                try
                {
                    return c.BuiltInCategory.ToString();
                }
                catch
                {
                    return BuiltInCategory.INVALID.ToString();
                }
            }
        }
        public string categoryType
        {
            get
            {
                try
                {
                    return c.CategoryType.ToString();
                }
                catch
                {
                    return CategoryType.Invalid.ToString();
                }
            }
        }
        public bool allowsBoundParameters
        {
            get
            {
                try
                {
                    return c.AllowsBoundParameters;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool canAddSubcategory
        {
            get
            {
                try
                {
                    return c.CanAddSubcategory;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool hasMaterialQuantities
        {
            get
            {
                try
                {
                    return c.HasMaterialQuantities;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool isCuttable
        {
            get
            {
                try
                {
                    return c.IsCuttable;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool isTagCategory
        {
            get
            {
                try
                {
                    return c.IsTagCategory;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool isVisibleInUi
        {
            get
            {
                try
                {
                    return c.IsVisibleInUI;
                }
                catch
                {
                    return false;
                }
            }
        }
        public string color
        {
            get
            {
                try
                {
                    return c.LineColor.ToString() ?? string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        public string material
        {
            get
            {
                try
                {
                    return c.Material.UniqueId;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        public double parent
        {
            get
            {
                try
                {
                    return c.Parent.Id.Value;
                }
                catch
                {
                    return -1;
                }
            }
        }
        public IEnumerable<double> subcategoriesId
        {
            get
            {
                if (c.SubCategories is not null || c.SubCategories?.Size > 0)
                {
                    foreach (Category c in c.SubCategories)
                    {
                        yield return c.Id.Value;
                    }
                }
            }
        }
    }
}
