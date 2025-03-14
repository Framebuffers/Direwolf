using Direwolf.Contracts;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Direwolf.Definitions
{
    public readonly record struct Wolfpack([property: JsonIgnore]IHowler Howler, string DocumentName = "", string FileOrigin = "", string DocumentVersion = "", bool WasCompleted = false, string DispatcherName = "Query")
    {
        public DateTime CreatedAt { get; init; } = DateTime.Now;
        public Guid GUID { get; init; } = Guid.NewGuid();
        public int ResultCount { get => Howler.Den.Count; }
        public Stack<Prey> Results => Howler.Den;
        public override string ToString()
        {
            return JsonSerializer.Serialize(new Dictionary<string, object>()
            {
                ["id"] = GUID,
                ["createdAt"] = CreatedAt,
                ["timeTaken"] = 0,
                ["resultCount"] = ResultCount,
                ["howlerName"] = Howler.GetType().Name,
                ["testName"] = DispatcherName,
                ["fileName"] = DocumentName,
                ["fileVersion"] = DocumentVersion,
                ["fileOrigin"] = FileOrigin,
                ["wasCompleted"] = WasCompleted,
                ["data"] = Howler.Den
            });
        }
    }


}
