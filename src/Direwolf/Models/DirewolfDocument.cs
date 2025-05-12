using Direwolf.Definitions;

namespace Direwolf.Models;

public record DirewolfDocument(
    int Id,
    Guid DocumentCreationId,
    string DocumentName,
    bool IsFamily
)
{
    public virtual List<DirewolfElement>? Elements { get; set; }
    public virtual List<DocumentWarnings>? Warnings { get; set; }
    public virtual ProjectInformation ProjectInformation { get; set; }
}