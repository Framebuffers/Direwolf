using Direwolf.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Direwolf.Definitions
{
    public readonly record struct Wolfpack([property: JsonIgnore]IHowler Howler, string DispatcherName = "Query")
    {
        public DateTime Timestamp { get; init; } = DateTime.Now;
        public Guid GUID { get; init; } = Guid.NewGuid();
        public int ResultCount { get => Howler.Den.Count; }
        public Stack<Catch> Results => Howler.Den;
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
