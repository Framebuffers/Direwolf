using System.Text.Json;

namespace Direwolf.Definitions;

/// <summary>
///     Wrapper for result data. Includes metadata for identification and performance statistics. This is the format that
///     direwolf-db uses to store test results.
/// </summary>
/// <param name="WasCompleted">True if the result within is a successful test, false if otherwise</param>
/// <param name="TimeTaken">Time, in seconds, taken to generate the results being stored on this WolfQueue</param>
public readonly record struct Wolfpack
{
    public string Name { get; init; }
    public DateTime CreatedAt { get; init; }
    public Guid Guid { get; init; }
    public bool WasCompleted { get; init; }
    public double TimeTaken { get; init; }
    public object Data { get; init; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(Data);
    }

    public bool Equals(Wolfpack? other)
    {
        return other.Value.Name == Name && other.Value.Guid == Guid;
    }
}