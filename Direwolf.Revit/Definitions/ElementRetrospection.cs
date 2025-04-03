using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions;

/// <summary>
///     Retrieves parameters from a Revit <see cref="Element" />
/// </summary>
/// <param name="Element">Revit Element</param>
public readonly record struct ElementRetrospection(Element Element)
{
    public double id => Element.Id.Value;
    public string uniqueId => Element.UniqueId;

    public Category? category
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

    public BuiltInCategory builtInCategory
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

    public double assemblyInstanceId
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

    public double createdPhaseId
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

    public double demolishedPhaseId
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

    public bool hasDesignOption
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

    public double groupId
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

    public double levelId
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

    public string location
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

    public string name
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

    public double ownerViewId
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

    public bool isPinned
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

    public bool isViewSpecific
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

    public double worksetId
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