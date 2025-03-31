using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Direwolf.Revit.Definitions;

namespace Direwolf.Revit.Utilities;
public static class DirewolfExtensions
{
    public static ICollection<Element>? _GetAllValidElements(this Document doc)
    {
        return new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .WhereElementIsViewIndependent()
                .ToElements();
    }
    public static Dictionary<string, object>? GetParameterValue(this Parameter p)
    {
        Dictionary<string, object> parameters = [];
        if (p.Definition is not null)
        {
            switch (p.StorageType)
            {
                case StorageType.Integer:
                    parameters.Add("storageType", StorageType.Integer.ToString());
                    parameters.Add("name", p.Definition.Name);
                    parameters.Add("value", p.AsInteger());
                    break;
                case StorageType.Double:
                    parameters.Add("storageType", StorageType.Double.ToString());
                    parameters.Add("name", p.Definition.Name);
                    parameters.Add("value", p.AsDouble());
                    break;
                case StorageType.String:
                    parameters.Add("storageType", StorageType.String.ToString());
                    parameters.Add("name", p.Definition.Name);
                    parameters.Add("value", p.AsString());
                    break;
                case StorageType.ElementId:
                    parameters.Add("storageType", StorageType.ElementId.ToString());
                    parameters.Add("name", p.Definition.Name);
                    parameters.Add("value", p.AsElementId().Value);
                    break;
                case StorageType.None:
                default:
                    break;
            }
            parameters.Add("isReadOnly", p.IsReadOnly);
            parameters.Add("typeId", p.GetTypeId().TypeId ?? string.Empty);
            parameters.Add("dataType", p.Definition.GetDataType().TypeId);
            parameters.Add("groupTypeId", p.Definition.GetGroupTypeId().TypeId ?? string.Empty);
            parameters.Add("hasValue", p.HasValue);
            parameters.Add("isShared", p.IsShared);
            parameters.Add("userModifiable", p.UserModifiable);

            if (p.IsShared)
            {
                parameters.Add("sharedParamGuid", p.GUID);
            }
            else
            {
                parameters.Add("sharedParamGuid", Guid.Empty.ToString());
            }
        }
        return parameters;
    }


}
