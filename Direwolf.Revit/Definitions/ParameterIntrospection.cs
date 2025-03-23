using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct ParameterIntrospection(Parameter parameter)
    {
        public double id { get; init; }
        public string? name { get; init; }
        public string? value { get; init; }
        public string? storageType { get; init; }
        public string? unitTypeId { get; init; }
        public string? parentElement { get; init; }
        public bool? hasValue { get; init; }
        public bool? userModifiable { get; init; }
        public bool? isShared { get; init; }
        public string? sharedParameterGuid { get; init; }
        public string? dataType { get; init; }
        public string? groupTypeId { get; init; }

    }
 //public double id
        //{
        //    get
        //    {
        //        try
        //        {
        //            return parameter.Id.Value;
        //        }
        //        catch
        //        {
        //            return -1;
        //        }
        //    }
        //}
        //public string? name
        //{
        //    get
        //    {
        //        try
        //        {
        //            return parameter.Definition.Name;
        //        }
        //        catch
        //        {
        //            return string.Empty;
        //        }
        //    }
        //}
        //public string? value
        //{
        //    get
        //    {
        //        try
        //        {
        //            return parameter.StorageType switch
        //            {
        //                StorageType.None => "None",
        //                StorageType.Integer => parameter.AsInteger().ToString(),
        //                StorageType.Double => parameter.AsDouble().ToString(),
        //                StorageType.String => parameter.AsString(),
        //                StorageType.ElementId => parameter.AsElementId().ToString(),
        //                _ => "None",
        //            };
        //        }
        //        catch
        //        {
        //            return string.Empty;
        //        }
        //    }
        //}
        //public string? storageType
        //{
        //    get
        //    {
        //        try
        //        {
        //            return parameter.StorageType.ToString();
        //        }
        //        catch
        //        {
        //            return StorageType.None.ToString();
        //        }
        //    }
        //}
        //public string? unitTypeId
        //{
        //    get
        //    {
        //        try
        //        {
        //            return parameter.GetUnitTypeId().TypeId.ToString();
        //        }
        //        catch
        //        {
        //            return string.Empty;
        //        }
        //    }
        //}
        //public string? parentElement
        //{
        //    get
        //    {
        //        try
        //        {
        //            return parameter.Element.UniqueId;
        //        }
        //        catch
        //        {
        //            return Guid.Empty.ToString();
        //        }
        //    }
        //}

        //public bool? hasValue
        //{
        //    get
        //    {
        //        try
        //        {
        //            return hasValue;
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }
        //}
        //public bool? userModifiable
        //{
        //    get
        //    {
        //        try
        //        {
        //            return userModifiable;
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }
        //}
        //public bool? isShared
        //{
        //    get
        //    {
        //        try
        //        {
        //            return parameter.IsShared;
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }
        //}
        //public string? sharedParameterGuid
        //{
        //    get
        //    {
        //        try
        //        {
        //            return parameter.GUID.ToString();
        //        }
        //        catch
        //        {
        //            return Guid.Empty.ToString();
        //        }
        //    }
        //}
        //public string? dataType
        //{
        //    get
        //    {
        //        try
        //        {
        //            return parameter.Definition.GetDataType().TypeId;
        //        }
        //        catch
        //        {
        //            return string.Empty;
        //        }
        //    }
        //}
        //public string? groupTypeId
        //{
        //    get
        //    {
        //        try
        //        {
        //            return parameter.Definition.GetGroupTypeId().TypeId;
        //        }
        //        catch
        //        {
        //            return string.Empty;
        //        }
        //    }
        //}
}
