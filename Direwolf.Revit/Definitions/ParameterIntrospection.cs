using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct ParameterIntrospection(Parameter Parameter)
    {
        public string name
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
        public string value
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
                        _ => "None",
                    };
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
    }
}
