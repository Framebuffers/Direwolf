using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions;

/// <summary>
///     Retrieves parameters from a Revit <see cref="Element" />
/// </summary>
/// <param name="Element">Revit Element</param>
public readonly record struct ElementRetrospection(Element Element)
{
    public double Id => Element.Id.Value;
    public string UniqueId => Element.UniqueId;

    public Category? Category
    {
        get
        {
            try
            {
                if (Element.Category is not null) return Element.Category;
            }
            catch
            {
                return null;
            }

            return null;
        }
    }

    public BuiltInCategory BuiltInCategory
    {
        get
        {
            try
            {
                if (Element.Category is not null) return Element.Category.BuiltInCategory;

                return BuiltInCategory.INVALID;
            }
            catch
            {
                return BuiltInCategory.INVALID;
            }
        }
    }

    public double AssemblyInstanceId
    {
        get
        {
            try
            {
                return Element.AssemblyInstanceId.Value;
            }
            catch
            {
                return -1;
            }
        }
    }

    public double CreatedPhaseId
    {
        get
        {
            try
            {
                return Element.CreatedPhaseId.Value;
            }
            catch
            {
                return -1;
            }
        }
    }

    public double DemolishedPhaseId
    {
        get
        {
            try
            {
                return Element.DemolishedPhaseId.Value;
            }
            catch
            {
                return -1;
            }
        }
    }

    public bool HasDesignOption
    {
        get
        {
            try
            {
                if (Element.DesignOption is not null) return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
    }

    public double GroupId
    {
        get
        {
            try
            {
                if (Element.GroupId.Value != -1 || Element.GroupId is null) return -1;
                return Element.GroupId.Value;
            }
            catch
            {
                return -1;
            }
        }
    }

    public double LevelId
    {
        get
        {
            try
            {
                return Element.LevelId.Value;
            }
            catch
            {
                return -1;
            }
        }
    }

    public string Location
    {
        get
        {
            try
            {
                if (Element.Location is not null) return Element.Location.ToString() ?? string.Empty;
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    public string Name
    {
        get
        {
            try
            {
                return Element.Name;
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    public double OwnerViewId
    {
        get
        {
            try
            {
                return Element.OwnerViewId.Value;
            }
            catch
            {
                return -1;
            }
        }
    }

    public bool IsPinned
    {
        get
        {
            try
            {
                return Element.Pinned;
            }
            catch
            {
                return false;
            }
        }
    }

    public bool IsViewSpecific
    {
        get
        {
            try
            {
                return Element.ViewSpecific;
            }
            catch
            {
                return false;
            }
        }
    }

    public double WorksetId
    {
        get
        {
            try
            {
                return Element.WorksetId.IntegerValue;
            }
            catch
            {
                return -1;
            }
        }
    }
}