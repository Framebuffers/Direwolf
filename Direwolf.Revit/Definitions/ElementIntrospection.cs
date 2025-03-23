using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct ElementIntrospection(Element Element)
    {
        //public double? id { get; init; }
        //public string? uniqueId { get; init; }
        //public string? elementVersionId { get; init; }
        //public string? documentOwnerId { get; init; }
        //public string? builtInCategory { get; init; }
        //public double? assemblyInstanceId { get; init; }
        //public double? createdPhaseId { get; init; }
        //public double? demolishedPhaseId { get; init; }
        //public bool? hasDesignOption { get; init; }
        //public double? groupId { get; init; }
        //public double? levelId { get; init; }
        //public string? location { get; init; }
        //public string? name { get; init; }
        //public double? ownerViewId { get; init; }
        //public bool? isPinned { get; init; }
        //public bool? isViewSpecific { get; init; }
        //public bool? isModifiable { get; init; }
        //public bool? isGrouped { get; init; }
        //public int? worksetId { get; init; }
        //public string? workshareId { get; init; }
        //public bool? isBuiltInCategory { get; init; }
        //public bool? isAnnotative { get; init; }
        //public bool? isModel { get; init; }
        //public bool? isWorkshared { get; init; }




        public double? id { get; init; }

        public bool? hasDesignOption { get; init; }
        public bool? isPinned { get; init; }
        public bool? isViewSpecific { get; init; }
        public bool? isModifiable { get; init; }
        public bool? isGrouped { get; init; }
        public bool? isBuiltInCategory { get; init; }
        public bool? isAnnotative { get; init; }
        public bool? isModel { get; init; }
        public bool? isWorkshared { get; init; }

        public double? assemblyInstanceId { get; init; }
        public double? createdPhaseId { get; init; }
        public double? demolishedPhaseId { get; init; }
        public double? groupId { get; init; }
        public double? levelId { get; init; }
        public double? ownerViewId { get; init; }

        public int? worksetId { get; init; }

        public string? uniqueId { get; init; }
        public string? elementVersionId { get; init; }
        public string? documentOwnerId { get; init; }
        public string? builtInCategory { get; init; }
        public string? location { get; init; }
        public string? name { get; init; }
        public string? workshareId { get; init; }

        public IList<ParameterIntrospection>? parameters { get; init; }

    }

}

  //public double? id
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Element.Id.Value;
        //        }
        //        catch
        //        {
        //            return -1;
        //        }
        //    }
        //}
        //public string? uniqueId
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Element.UniqueId;
        //        }
        //        catch
        //        {
        //            return Guid.Empty.ToString();
        //        }
        //    }
        //}
        //public Category? category
        //{
        //    get
        //    {
        //        try
        //        {
        //            if (Element.Category is not null)
        //            {
        //                return Element.Category;
        //            }
        //        }
        //        catch
        //        {
        //            return null;
        //        }
        //        return null;
        //    }
        //}
        //public string? builtInCategory
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Element.Category.BuiltInCategory.ToString();
        //        }
        //        catch
        //        {
        //            return BuiltInCategory.INVALID.ToString();
        //        }
        //    }
        //}
        //public double? assemblyInstanceId
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Element.AssemblyInstanceId.Value;
        //        }
        //        catch
        //        {
        //            return -1;
        //        }
        //    }
        //}
        //public double? createdPhaseId
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Element.CreatedPhaseId.Value;
        //        }
        //        catch
        //        {
        //            return -1;
        //        }
        //    }
        //}
        //public double? demolishedPhaseId
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Element.DemolishedPhaseId.Value;
        //        }
        //        catch
        //        {
        //            return -1;
        //        }
        //    }

        //}
        //public bool? hasDesignOption
        //{
        //    get
        //    {
        //        try
        //        {
        //            if (Element.DesignOption is not null) return true;
        //            else return false;
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }
        //}
        //public double? groupId
        //{
        //    get
        //    {
        //        try
        //        {
        //            if (Element.GroupId.Value != -1 || Element.GroupId is null) return -1;
        //            else return Element.GroupId.Value;
        //        }
        //        catch
        //        {
        //            return -1;
        //        }
        //    }
        //}
        //public double? levelId
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Element.LevelId.Value;
        //        }
        //        catch
        //        {
        //            return -1;
        //        }
        //    }
        //}
        //public string? location
        //{
        //    get
        //    {
        //        try
        //        {
        //            if (Element.Location is not null) return Element.Location.ToString() ?? string?.Empty;
        //            else return string?.Empty;
        //        }
        //        catch
        //        {
        //            return string?.Empty;
        //        }
        //    }

        //}
        //public string? name
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Element.Name;
        //        }
        //        catch
        //        {
        //            return string?.Empty;
        //        }
        //    }
        //}
        //public double? ownerViewId
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Element.OwnerViewId.Value;
        //        }
        //        catch
        //        {
        //            return -1;
        //        }
        //    }
        //}
        //public bool? isPinned
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Element.Pinned;
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }
        //}
        //public bool? isViewSpecific
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Element.ViewSpecific;
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }
        //}
        //public double? worksetId
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Element.WorksetId.IntegerValue;
        //        }
        //        catch
        //        {
        //            return -1;
        //        }
        //    }
        //}






