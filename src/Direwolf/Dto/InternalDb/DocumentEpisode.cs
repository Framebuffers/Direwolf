using Autodesk.Revit.DB;

namespace Direwolf.Dto.InternalDb;

public readonly record struct DocumentEpisode(string DocumentSaveId, int SaveCount)
{
    public static DocumentEpisode Create(Document doc)
    {
        return new DocumentEpisode(Document.GetDocumentVersion(doc).VersionGUID.ToString(),
                                   Document.GetDocumentVersion(doc).NumberOfSaves);
    }
}