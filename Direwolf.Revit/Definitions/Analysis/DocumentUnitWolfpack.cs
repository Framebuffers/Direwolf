using Autodesk.Revit.DB;
using Direwolf.Revit.Contracts;
using Direwolf.Revit.Definitions.Primitives;

namespace Direwolf.Revit.Definitions.Analysis;

/// <summary>
///     Retrieves the Units set on a given Revit Document.
/// </summary>
public readonly record struct DocumentUnitWolfpack : IRevitWolfpack
{
    /// <summary>
    ///     Retrieves the Units set on a given Revit Document.
    /// </summary>
    /// <param name="document">Revit Document</param>
    private DocumentUnitWolfpack(Document document)
    {
        Document = document;
        Name = document.Title ?? string.Empty;
        RevitDocument = new RevitDocumentId(document);
    }

    private Dictionary<string, object> Units =>
        new()
        {
            ["Volume"] = Document.GetUnits().GetFormatOptions(SpecTypeId.Volume).GetUnitTypeId().TypeId,
            ["LengthUnits"] = Document.GetUnits().GetFormatOptions(SpecTypeId.Length).GetUnitTypeId().TypeId,
            ["AreaUnits"] = Document.GetUnits().GetFormatOptions(SpecTypeId.Area).GetUnitTypeId().TypeId,
            ["Angle"] = Document.GetUnits().GetFormatOptions(SpecTypeId.Angle).GetUnitTypeId().TypeId,
            ["Currency"] = Document.GetUnits().GetFormatOptions(SpecTypeId.Currency).GetUnitTypeId().TypeId,
            ["Number"] = Document.GetUnits().GetFormatOptions(SpecTypeId.Number).GetUnitTypeId().TypeId,
            ["RotationAngle"] =
                Document.GetUnits().GetFormatOptions(SpecTypeId.RotationAngle).GetUnitTypeId().TypeId,
            ["SheetLength"] = Document.GetUnits().GetFormatOptions(SpecTypeId.SheetLength).GetUnitTypeId().TypeId,
            ["SiteAngle"] = Document.GetUnits().GetFormatOptions(SpecTypeId.SiteAngle).GetUnitTypeId().TypeId,
            ["Slope"] = Document.GetUnits().GetFormatOptions(SpecTypeId.Slope).GetUnitTypeId().TypeId,
            ["Speed"] = Document.GetUnits().GetFormatOptions(SpecTypeId.Speed).GetUnitTypeId().TypeId,
            ["Time"] = Document.GetUnits().GetFormatOptions(SpecTypeId.Time).GetUnitTypeId().TypeId
        };

    private Document Document { get; }

    /// <summary>
    ///     <inheritdoc cref="Wolfpack.Guid" />
    /// </summary>
    public Guid Guid { get; } = Guid.NewGuid();

    /// <summary>
    ///     <inheritdoc cref="Wolfpack.Name" />
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     <inheritdoc cref="Wolfpack.CreatedAt" />
    /// </summary>
    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    /// <summary>
    ///     <inheritdoc cref="Wolfpack.WasCompleted" />
    /// </summary>
    public bool WasCompleted { get; }

    /// <summary>
    ///     <inheritdoc cref="Direwolf.TimeTaken" />
    /// </summary>
    public double TimeTaken { get; }

    /// <summary>
    ///     A <see cref="RevitDocumentId" /> joins both the unique identifiers of a document (its CreationGUID)
    ///     alongside identifiers from a specific "episode" (its VersionGUID and save count number).
    ///     Two RevitDocumentId are equal when they match in CreationGUID.
    /// </summary>
    public RevitDocumentId RevitDocument { get; init; }

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
                return Units;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public static DocumentUnitWolfpack CreateInstance(Document document)
    {
        return new DocumentUnitWolfpack(document);
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