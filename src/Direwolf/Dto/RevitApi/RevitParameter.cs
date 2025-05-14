using Autodesk.Revit.DB;

namespace Direwolf.Dto.RevitApi;

public readonly record struct RevitParameter(
    string DataType,
    string Key,
    string Value)
{
    public static RevitParameter Create(Parameter p)
    {
        return new RevitParameter(
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