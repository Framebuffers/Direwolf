using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Autodesk.Revit.DB;

namespace Direwolf.Definitions;

public readonly record struct ParameterData(
    string DataType,
    string Key, 
    string Value)
{
    public static ParameterData Create(Parameter p)
    {
        return new ParameterData(
            p.StorageType.ToString(),
            p.Definition.Name,
            GetValue(p)
        );
    }
    
    private static string GetValue(Parameter p)
    {
        return p.StorageType switch
        {
            StorageType.None => "None",
            StorageType.Integer => p.AsInteger().ToString(),
            StorageType.Double => p.AsDouble().ToString(),
            StorageType.String => p.AsString(),
            StorageType.ElementId => p.AsElementId().ToString(),
            _ => "None"
        };
    }
}