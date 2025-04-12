using Direwolf.Contracts;

namespace Direwolf.Primitives;

/// <summary>
///     Wrapper for result data. Includes metadata for identification and performance statistics. This is the format that
///     direwolf-db uses to store test results.
/// </summary>
/// <param name="WasCompleted">True if the result within is a successful test, false if otherwise</param>
/// <param name="TimeTaken">Time, in seconds, taken to generate the results being stored on this WolfQueue</param>
public readonly record struct Wolfpack : IWolfpack
{
    private Wolfpack(string? name, bool wasCompleted, double timeTaken, object? data)
    {
        Guid = Guid.NewGuid();
        Name = name ?? string.Empty;
        CreatedAt = DateTime.UtcNow;
        WasCompleted = wasCompleted;
        TimeTaken = timeTaken;
        Data = data;
    }

    /// <summary>
    ///     Unique identifier for this query.
    /// </summary>
    public Guid Guid { get; init; }

    /// <summary>
    ///     Unique identifier for this query.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    ///     Creation date of this query.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    ///     Indicator to store if a query is valid or has been completed.
    ///     It serves as a quickly-indexable value to check for valid results.
    /// </summary>
    public bool WasCompleted { get; init; }

    /// <summary>
    ///     Time taken to perform this query and serialize its results.
    /// </summary>
    public double TimeTaken { get; init; }

    /// <summary>
    ///     Results from a query.
    /// </summary>
    public object? Data { get; init; }

    public static Wolfpack New(string? name, object? data, bool wasCompleted = false, double timeTaken = 0)
    {
        return new Wolfpack(name, wasCompleted, timeTaken, data);
    }

    /// <summary>
    ///     A RevitWolfpack is designed to be easily serialized to any format: JSON, XLSX, CSV--
    ///     But for that, we need to make the process as easy as possible.
    ///     For this, the metadata is stripped from the serialization string and only the data stored
    ///     is serialized when calling <see cref="ToString" />.
    /// </summary>
    /// <returns>The results held inside Data as string.</returns>
    public override string ToString()
    {
        return Data?.ToString() ?? string.Empty;
    }
}