using Autodesk.Revit.DB;

namespace Direwolf.Dto.InternalDb;

public readonly record struct DocumentId
{
    public string DocumentName { get; init; }
    public string DocumentUniqueId { get; init; }
    public DocumentType DocumentType { get; init; }
}