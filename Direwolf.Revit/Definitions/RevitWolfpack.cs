using Autodesk.Revit.DB;
using Direwolf.Contracts;
using Direwolf.Definitions;

namespace Direwolf.Revit.Definitions;

/// <inheritdoc />
public interface IRevitWolfpack : IWolfpack
{
    public RevitDocumentId RevitDocument { get; init; }
}

/// <inheritdoc />
public readonly record struct RevitWolfpack : IRevitWolfpack
{
    private RevitWolfpack(Document doc, bool wasCompleted = false, double timeTaken = 0, object data = null)
    {
        Guid = Guid.NewGuid();
        Name = doc.Title ?? string.Empty;
        RevitDocument = new RevitDocumentId(doc);
        CreatedAt = DateTime.UtcNow;
        WasCompleted = wasCompleted;
        TimeTaken = timeTaken;
        Data = data;
    }

    public static RevitWolfpack CreateInstance(Document doc,
        bool wasCompleted = false,
        double timeTaken = 0,
        object data = null)
    {
        return new RevitWolfpack(doc, wasCompleted, timeTaken, data);
    }

    /// <summary>
    /// <inheritdoc cref="Wolfpack.Guid"/>
    /// </summary>
    public Guid Guid { get; init; }

    /// <summary>
    /// <inheritdoc cref="Wolfpack.Name"/>
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// <inheritdoc cref="Wolfpack.CreatedAt"/>
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// <inheritdoc cref="Wolfpack.WasCompleted"/>
    /// </summary>
    public bool WasCompleted { get; init; }

    /// <summary>
    /// <inheritdoc cref="Direwolf.TimeTaken"/>
    /// </summary>
    public double TimeTaken { get; init; }

    /// <summary>
    /// A <see cref="RevitDocumentId"/> joins both the unique identifiers of a document (its CreationGUID)
    /// alongside identifiers from a specific "episode" (its VersionGUID and save count number).
    /// Two RevitDocumentId are equal when they match in CreationGUID.
    /// </summary>
    public RevitDocumentId RevitDocument { get; init; }

    /// <summary>
    /// Implements the <see cref="IWolfpack.Data"/> part of the interface. For a RevitWolfpack, there's a main
    /// distinction: it redirects the Data object to a Dictionary called Results. Therefore, it is just a stub to get
    /// Results data.
    /// </summary>
    public object? Data { get; init; }
}