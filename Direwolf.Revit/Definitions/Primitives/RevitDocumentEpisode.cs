using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions.Primitives;

public readonly record struct RevitDocumentEpisode
{
    public RevitDocumentEpisode(Document doc)
    {
        Id = Guid.NewGuid();
        DocumentName = doc.Title;
        DocumentUniqueId = doc.CreationGUID;
        DocumentSaveId = Document.GetDocumentVersion(doc).VersionGUID;
        DocumentSaveCount = Document.GetDocumentVersion(doc).NumberOfSaves;
    }

    public Guid Id { get; init; }
    public string DocumentName { get; init; }
    private Guid DocumentUniqueId { get; }
    public Guid DocumentSaveId { get; init; }
    public int DocumentSaveCount { get; init; }

    public bool Equals(RevitDocumentEpisode? other)
    {
        if (other is null) return false;
        return other.Value.DocumentUniqueId == DocumentUniqueId;
    }
}