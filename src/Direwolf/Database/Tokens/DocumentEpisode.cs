using Autodesk.Revit.DB;

namespace Direwolf.Database.Tokens;

public readonly record struct DocumentEpisode(
    string DocumentSaveId,
    int SaveCount
    )
{
    public static DocumentEpisode Create(Document doc) => new(
        Document.GetDocumentVersion(doc).VersionGUID.ToString(),
        Document.GetDocumentVersion(doc).NumberOfSaves
        );
}