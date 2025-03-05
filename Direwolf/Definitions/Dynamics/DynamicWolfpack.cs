using Direwolf.Contracts.Dynamics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Direwolf.Definitions.Dynamics
{
    public readonly record struct DynamicWolfpack([property: JsonIgnore] IDynamicHowler Howler, string DispatcherName = "Query")
    {
        public DateTime Timestamp { get; init; } = DateTime.Now;
        public Guid GUID { get; init; } = Guid.NewGuid();
        public int ResultCount { get => Howler.Den.Count; }
        public Stack<DynamicCatch> Results => Howler.Den;
        public override string ToString()
        {
            return JsonSerializer.Serialize(new Dictionary<string, object>()
            {
                ["DateTime"] = Timestamp,
                ["GUID"] = GUID,
                ["ResultCount"] = ResultCount,
                ["Results"] = Howler
            });
        }
    }


}
