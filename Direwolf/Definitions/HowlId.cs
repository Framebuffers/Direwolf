namespace Direwolf.Definitions
{
    /// <summary>
    /// Unique identifier for each query.
    /// </summary>
    /// <param name="HowlIdentifier">A unique identification for a performed query</param>
    /// <param name="Name">Human-readable name for the query</param>
    public readonly record struct HowlId(Guid HowlIdentifier, string Name)
    {
    }
}
