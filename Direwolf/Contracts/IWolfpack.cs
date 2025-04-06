namespace Direwolf.Contracts;

public interface IWolfpack
{
    /// <summary>
    /// Unique identifier for this query.
    /// </summary>
    public Guid Guid { get; }

    /// <summary>
    /// The name of a query.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Creation date of this query.
    /// </summary>
    public DateTime CreatedAt { get; }

    /// <summary>
    /// Indicator to store if a query is valid or has been completed.
    /// It serves as a quickly-indexable value to check for valid results.
    /// </summary>
    public bool WasCompleted { get; }

    /// <summary>
    /// Time taken to perform this query and serialize its results.
    /// </summary>
    public double TimeTaken { get; }

    /// <summary>
    /// Results from a query.
    /// </summary>
    public object? Data { get; }
}