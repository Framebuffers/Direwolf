using System.Dynamic;
using System.Text.Json.Serialization;

namespace Direwolf.Definitions.Dynamics
{
    public readonly record struct DynamicCatch([property: JsonExtensionData] ExpandoObject Result) { }
  }
