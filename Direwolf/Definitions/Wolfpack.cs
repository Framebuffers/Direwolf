using System.Text.Json;
using System.Text.Json.Serialization;
using Direwolf.Contracts;

namespace Direwolf.Definitions;

/// <summary>
///     Wrapper for result data. Includes metadata for identification and performance statistics. This is the format that
///     direwolf-db uses to store test results.
/// </summary>
/// <param name="Howler">Dispatcher containing the results</param>
/// <param name="DocumentName">Source name</param>
/// <param name="FileOrigin">Source path</param>
/// <param name="DocumentVersion">UUID of the current document revision</param>
/// <param name="WasCompleted">True if the result within is a successful test, false if otherwise</param>
/// <param name="TimeTaken">Time, in seconds, taken to generate the results being stored on this WolfQueue</param>
public readonly record struct Wolfpack(
    [property: JsonIgnore] IHowler Howler,
    string DocumentName = "",
    string FileOrigin = "",
    string DocumentVersion = "",
    bool WasCompleted = false,
    double TimeTaken = 0)
{
    public string TestName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public Guid Guid { get; } = Guid.NewGuid();
    public int ResultCount => Howler.Den.Count;
    public string? Results => Howler?.ToString();
    public override string ToString()
    {
        return JsonSerializer.Serialize(new Dictionary<string, object>
        {
            ["id"] = Guid,
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