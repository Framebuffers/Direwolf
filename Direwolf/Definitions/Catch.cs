using System.Text.Json.Serialization;

namespace Direwolf.Definitions
{
    public readonly record struct Catch([property: JsonExtensionData] Dictionary<string, object> Result) { }
  }
