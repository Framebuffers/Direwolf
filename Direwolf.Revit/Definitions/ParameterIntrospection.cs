using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions;

/// <summary>
///     Retrieves the name and value of a given Revit Parameter.
/// </summary>
/// <param name="Parameter">The Parameter of a Revit Element</param>
public readonly record struct ParameterIntrospection(Parameter Parameter)
{
    public string Name
    {
        get
        {
            try
            {
                return Parameter.Definition.Name;
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    public string Value
    {
        get
        {
            try
            {
                return Parameter.StorageType switch
                {
                    StorageType.None => "None",
                    StorageType.Integer => Parameter.AsInteger().ToString(),
                    StorageType.Double => Parameter.AsDouble().ToString(),
                    StorageType.String => Parameter.AsString(),
                    StorageType.ElementId => Parameter.AsElementId().ToString(),
                    _ => "None"
                };
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}