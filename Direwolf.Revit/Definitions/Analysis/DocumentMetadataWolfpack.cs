using Autodesk.Revit.DB;
using Direwolf.Contracts;
using Direwolf.Revit.Definitions.Primitives;

namespace Direwolf.Revit.Definitions.Analysis;

/// <summary>
///     Retrieves document metadata from a Revit document.
/// </summary>
/// <param name="Document">Revit document</param>
public readonly record struct DocumentMetadataWolfpack : IWolfpack
{
    /// <summary>
    ///     Retrieves the Units set on a given Revit Document.
    /// </summary>
    /// <param name="document">Revit Document</param>
    private DocumentMetadataWolfpack(Document document)
    {
        Document = document;
        Name = document.Title ?? string.Empty;
        RevitDocument = new RevitDocumentEpisode(document);
    }

    /// <summary>
    ///     A <see cref="RevitDocumentEpisode" /> joins both the unique identifiers of a document (its CreationGUID)
    ///     alongside identifiers from a specific "episode" (its VersionGUID and save count number).
    ///     Two RevitDocumentEpisode are equal when they match in CreationGUID.
    /// </summary>
    public RevitDocumentEpisode RevitDocument { get; init; }

    private Dictionary<string, object> DocumentData =>
        new()
        {
            ["DocumentName"] = Document.Title,
            ["DocumentPath"] = Document.PathName,
            ["DocumentUniqueId"] = Document.CreationGUID.ToString(),
            ["DocumentVersionId"] = Document.GetDocumentVersion(Document).VersionGUID.ToString(),
            ["DocumentSaveCount"] = Document.GetDocumentVersion(Document).NumberOfSaves,
            ["Warnings"] = Document.GetWarnings().Count,
            ["ActiveWorkset"] = Document.GetWorksetTable().GetActiveWorksetId().IntegerValue
        };

    private Document Document { get; }

    /// <summary>
    ///     <inheritdoc cref="Wolfpack.Guid" />
    /// </summary>
    public Guid Guid { get; init; } = Guid.NewGuid();

    /// <summary>
    ///     <inheritdoc cref="Wolfpack.Name" />
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    ///     <inheritdoc cref="Wolfpack.CreatedAt" />
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    ///     <inheritdoc cref="Wolfpack.WasCompleted" />
    /// </summary>
    public bool WasCompleted { get; init; } = false;

    /// <summary>
    ///     <inheritdoc cref="Direwolf.TimeTaken" />
    /// </summary>
    public double TimeTaken { get; init; } = 0;

    /// <summary>
    ///     Implements the <see cref="IWolfpack.Data" /> part of the interface. For a RevitWolfpack, there's a main
    ///     distinction: it redirects the Data object to a Dictionary called Results. Therefore, it is just a stub to get
    ///     Results data.
    /// </summary>
    public object Data
    {
        get
        {
            try
            {
                return DocumentData;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public static DocumentMetadataWolfpack CreateInstance(Document document)
    {
        return new DocumentMetadataWolfpack(document);
    }

    public void Deconstruct(out Document document)
    {
        document = Document;
    }

    public override string ToString()
    {
        return Data.ToString() ?? string.Empty;
    }
}