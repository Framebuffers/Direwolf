using System.Text.Json.Serialization;

namespace Direwolf.Definitions
{
    public readonly record struct Prey([property: JsonExtensionData] Dictionary<string, object> Result) { }
  }
