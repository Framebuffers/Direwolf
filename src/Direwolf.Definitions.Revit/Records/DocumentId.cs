using Autodesk.Revit.DB;

namespace Direwolf.Definitions.PlatformSpecific.Records;

// Unimplemented feature as of 2025-05-29
public readonly record struct DocumentId
{
    public string DocumentName { get; init; }
    public string DocumentUniqueId { get; init; }
    public DocumentType DocumentType { get; init; }
}