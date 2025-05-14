using Direwolf.Dto.InternalDb.Enums;

namespace Direwolf.Dto.Parser;

public record DirewolfYamlRecord(
    string WolfName,
    DocumentType DocumentType
)
{
    public virtual List<Set> Sets { get; init; } = [];
    public virtual List<Target> Targets { get; init; } = [];
}