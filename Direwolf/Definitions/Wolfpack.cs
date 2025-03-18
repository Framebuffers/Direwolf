using Direwolf.Contracts;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Direwolf.Definitions
{
    public readonly record struct Wolfpack([property: JsonIgnore]IHowler Howler, string DocumentName = "", string FileOrigin = "", string DocumentVersion = "", bool WasCompleted = false, double TimeTaken = 0)
    {
        public string TestName { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public Guid GUID { get; init; } = Guid.NewGuid();
        public int ResultCount { get => Howler.Den.Count; }
        public string? Results => Howler?.ToString();
        public override string ToString()
        {
            return JsonSerializer.Serialize(new Dictionary<string, object>()
            {
                ["id"] = GUID,
                ["createdAt"] = CreatedAt,
                ["timeTaken"] = TimeTaken,
                ["resultCount"] = ResultCount,
                ["howlerName"] = TestName,
                ["fileName"] = DocumentName,
                ["fileVersion"] = DocumentVersion,
                ["fileOrigin"] = FileOrigin,
                ["wasCompleted"] = WasCompleted,
                ["data"] = Howler.Den
            });
        }
    }
}
