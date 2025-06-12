using Autodesk.Revit.DB;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.Parsers;
using Direwolf.Definitions.RevitApi;

#pragma warning disable VISLIB0001
namespace Direwolf.Definitions;

/// <summary>
///     The base Data Operation Object type inside Direwolf. Holds any operation
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
            Payload = payload
        };
    }

    public static WolfpackCollectionLegacy ExportCategory(Document doc, string name, Method method,
        string[] elementUniqueIds,
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