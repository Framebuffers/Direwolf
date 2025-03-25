
namespace Direwolf.Definitions
{
    public interface IWolfpack
    {
        DateTime CreatedAt { get; init; }
        string FileOrigin { get; init; }
        Guid GUID { get; init; }
        int ResultCount { get; }
        string? Results { get; }
        string TestName { get; init; }
        double TimeTaken { get; init; }
        bool WasCompleted { get; init; }

        int GetHashCode();
        string ToString();
    }
}