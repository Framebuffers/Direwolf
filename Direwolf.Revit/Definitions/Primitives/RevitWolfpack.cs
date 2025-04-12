using Autodesk.Revit.DB;
using Direwolf.Contracts;
using Direwolf.Primitives;
using Direwolf.Revit.Contracts;

namespace Direwolf.Revit.Definitions.Primitives;

public readonly record struct RevitWolfpack : IRevitWolfpack
{
    private RevitWolfpack(string? name,
        Document doc,
        bool wasCompleted = false,
        double timeTaken = 0,
        object? data = null)
    {
        Guid = Guid.NewGuid();
        Name = name ?? string.Empty;
        RevitDocument = new RevitDocumentId(doc);
        CreatedAt = DateTime.UtcNow;
        WasCompleted = wasCompleted;
        TimeTaken = timeTaken;
        Data = data;
    }

    private RevitWolfpack(IWolfpack wolfpack, Document doc)
    {
        Guid = wolfpack.Guid;
        Name = wolfpack.Name;
        RevitDocument = new RevitDocumentId(doc);
        CreatedAt = wolfpack.CreatedAt;
        WasCompleted = wolfpack.WasCompleted;
        TimeTaken = wolfpack.TimeTaken;
        Data = wolfpack.Data;
    }

    /// <summary>
    ///     <inheritdoc cref="Wolfpack.Guid" />
    /// </summary>
    public Guid Guid { get; init; }

    /// <summary>
    ///     <inheritdoc cref="Wolfpack.Name" />
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    ///     <inheritdoc cref="Wolfpack.CreatedAt" />
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    ///     <inheritdoc cref="Wolfpack.WasCompleted" />
    /// </summary>
    public bool WasCompleted { get; init; }

    /// <summary>
    ///     <inheritdoc cref="Direwolf.TimeTaken" />
    /// </summary>
    public double TimeTaken { get; init; }

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
    public object? Data { get; init; }

    public static RevitWolfpack New(string? name,
        Document doc,
        object? data,
        double timeTaken = 0,
        bool wasCompleted = false)
    {
        return new RevitWolfpack(name, doc, wasCompleted, timeTaken, data);
    }

    public static RevitWolfpack New(IWolfpack wolfpack, Document doc)
    {
        return new RevitWolfpack(wolfpack, doc);
    }
}