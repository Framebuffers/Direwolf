using Autodesk.Revit.DB;

namespace Direwolf.Definitions.Internal;

public readonly record struct DocumentEpisode(
    string DocumentSaveId,
    int    SaveCount)
{
    public static DocumentEpisode Create(Document doc)
    {
        return new DocumentEpisode(
            Document.GetDocumentVersion(doc).VersionGUID.ToString(),
            Document.GetDocumentVersion(doc).NumberOfSaves);
    }
}