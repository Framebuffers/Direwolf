using Direwolf.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Direwolf.Definitions
{
    public readonly record struct Wolfpack(IHowler Howler, string QueryName = "Query")
    {
        public DateTime QueryTimestamp { get; init; } = DateTime.Now;
        public Guid QueryIdentifier { get; init; } = Guid.NewGuid();
        public int ResultCount { get => Howler.Den.Count; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(new Dictionary<string, object>()
            {
                ["DateTime"] = QueryTimestamp,
                ["ID"] = QueryIdentifier,
                ["ResultCount"] = ResultCount,
                ["Results"] = JsonSerializer.Serialize(Howler.Den)
            });
        }
    }
}
