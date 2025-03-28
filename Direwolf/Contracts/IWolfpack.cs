
namespace Direwolf.Definitions
{
    public interface IWolfpack
    {
        Guid WolfpackUniqueId { get; init; }
        DateTime CreatedAt { get; init; }
        string? Name { get; init; }
        double? TimeTaken { get; init; }
        string? Source { get; init; }
        bool? WasCompleted { get; init; }
        int?  ResultCount { get; init; }
        string? Result { get; init; }
        int GetHashCode();
        string ToString();
    }
}