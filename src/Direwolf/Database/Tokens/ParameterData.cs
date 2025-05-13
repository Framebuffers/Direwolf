using Autodesk.Revit.DB;

namespace Direwolf.Database.Tokens;

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