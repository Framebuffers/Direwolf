using Autodesk.Revit.DB;

namespace Direwolf.Definitions.Internal;

/// <summary>
///     An episode is an identifier for the state of a <see cref="Document" /> that can both uniquely identify it, and
///     identify the specific save point it was generated at.
///     Episodes are used as a snapshot of the <see cref="Document" />'s state a <see cref="Autodesk.Revit.DB.Element" />
///     has been captured at.
/// </summary>
/// <param name="DocumentUniqueId">The <see cref="Document" /> CreationGUID.</param>
/// <param name="DocumentSaveId">The unique GUID created at the moment of saving this file.</param>
/// <param name="SaveCount">The number of saves this <see cref="Document" /> has.</param>
public readonly record struct DocumentEpisode(string DocumentUniqueId, string DocumentSaveId, int SaveCount)
{
    public static DocumentEpisode Create(Document doc)
    {
        return new DocumentEpisode(doc.CreationGUID.ToString(),
            Document.GetDocumentVersion
                    (doc)
                .VersionGUID.ToString(),
            Document.GetDocumentVersion
                    (doc)
                .NumberOfSaves);
        
    }
}