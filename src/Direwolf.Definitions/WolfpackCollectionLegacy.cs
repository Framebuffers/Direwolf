using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB;
using Direwolf.Definitions.Drivers;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.Internal;
using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.Parser;
using Direwolf.Definitions.RevitApi;

#pragma warning disable VISLIB0001
namespace Direwolf.Definitions;

/// <summary>
///     The base Payload Operation Object type inside Direwolf. Holds any operation
///     generated to and/or from an external source.
/// </summary>
/// <param name="Id">Collision-Resistant Unique Identifier.</param>
/// <param name="Name">Name of the query.</param>
/// <param name="Method">Context of which this query has been executed on.</param>
public record WolfpackCollectionLegacy(
    Cuid Id,
    string Name,
    Method Method,
    string DocumentFingerprint,
    string DocumentSaveStateId)
{
    public DateTime UtcCreatedAt { get; init; } = DateTime.UtcNow;
    public Dictionary<string, object>? Payload { get; init; }
    public List<string>? ElementUniqueIds { get; init; }

    public static WolfpackCollectionLegacy Create(Document doc, string name, Method method,
        IEnumerable<RevitElement?> results, Dictionary<string, object>? payload = null)
    {
        var docId = (doc.GetDocumentVersionCounter(), doc.GetDocumentUuidHash());
        return new WolfpackCollectionLegacy(Cuid.CreateRevitId(doc, out docId), name, method, doc.GetDocumentUuidHash(),
            doc.GetDocumentVersionCounter())
        {
            Payload = payload,
        };
    }

    public static WolfpackCollectionLegacy ExportCategory(Document doc, string name, Method method, string[] elementUniqueIds,
        BuiltInCategory[] categories, Dictionary<string, object>? additionalPayload = null)
    {
        var elements = new List<RevitElement>();
   
        elements.AddRange(doc.GetRevitDatabaseAsCacheItems()
            .Where(element => element is not null)
            .Select(element => (element, rvtElement: (RevitElement)element!.Value))
            .Where(t => categories.Contains(t.rvtElement.BuiltInCategory))
            .Select(t => t.rvtElement));
        var docId = (doc.GetDocumentVersionCounter(), doc.GetDocumentUuidHash());
        return new WolfpackCollectionLegacy(
            Cuid.CreateRevitId(doc, out docId),
            name,
            method,
            doc.GetDocumentUuidHash(),
            doc.GetDocumentVersionCounter())
        {
            Payload = additionalPayload
        };
    }
}

public readonly record struct Wolfpack(
    Cuid? Id,
    Method Method,
    string? Name,
    string? Description,
    ReadOnlyDictionary<string, string>? Properties,
    IReadOnlyList<Howl> Payload)
{
    public static Wolfpack CreateInstance(Cuid? id,
        Method method,
        string? name,
        string? description,
        ReadOnlyDictionary<string, string>? properties,
        IReadOnlyList<Howl> payload)
    {
        var wp = new Wolfpack(id, method, name, description, properties, payload);  
        WolfpackCreated?.Invoke(wp, EventArgs.Empty);
        return wp;
    }

    public static event EventHandler? WolfpackCreated;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    
};