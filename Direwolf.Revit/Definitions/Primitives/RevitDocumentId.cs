using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions.Primitives;

public readonly record struct RevitDocumentId
{
    public RevitDocumentId(Document doc)
    {
        Id = Guid.NewGuid();
        DocumentName = doc.Title;
        DocumentUniqueId = doc.CreationGUID;
        DocumentEpisode = Document.GetDocumentVersion(doc).VersionGUID;
        DocumentSaveCount = Document.GetDocumentVersion(doc).NumberOfSaves;
    }

    public Guid Id { get; init; }
    public string DocumentName { get; init; }
    private Guid DocumentUniqueId { get; }
    public Guid DocumentEpisode { get; init; }
    public int DocumentSaveCount { get; init; }

    public bool Equals(RevitDocumentId? other)
    {
        return other.Value.DocumentUniqueId == DocumentUniqueId;
    }
}