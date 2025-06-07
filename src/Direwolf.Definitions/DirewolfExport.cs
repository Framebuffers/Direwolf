using Autodesk.Revit.DB;
using Direwolf.Definitions.Drivers;
using Direwolf.Definitions.Extensions;
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
/// <param name="Realm">Context of which this query has been executed on.</param>
public record DirewolfExport(
    Cuid Id,
    string Name,
    Realm Realm,
    string DocumentFingerprint,
    string DocumentSaveStateId)
{
    public DateTime UtcCreatedAt { get; init; } = DateTime.UtcNow;
    public Dictionary<string, object>? Payload { get; init; }
    public List<string>? ElementUniqueIds { get; init; }

    public static DirewolfExport Create(Document doc, string name, Realm realm,
        IEnumerable<RevitElement?> results, Dictionary<string, object>? payload = null)
    {
        var docId = (doc.GetDocumentVersionCounter(), doc.GetDocumentUuidHash());
        return new DirewolfExport(Cuid.CreateRevitId(doc, out docId), name, realm, doc.GetDocumentUuidHash(),
            doc.GetDocumentVersionCounter())
        {
            Payload = payload,
        };
    }

    public static DirewolfExport ExportCategory(Document doc, string name, Realm realm, string[] elementUniqueIds,
        BuiltInCategory[] categories, Dictionary<string, object>? additionalPayload = null)
    {
        var elements = new List<RevitElement>();
   
        elements.AddRange(doc.GetRevitDatabaseAsCacheItems()
            .Where(element => element is not null)
            .Select(element => (element, rvtElement: (RevitElement)element!.Value))
            .Where(t => categories.Contains(t.rvtElement.BuiltInCategory))
            .Select(t => t.rvtElement));
        var docId = (doc.GetDocumentVersionCounter(), doc.GetDocumentUuidHash());
        return new DirewolfExport(
            Cuid.CreateRevitId(doc, out docId),
            name,
            realm,
            doc.GetDocumentUuidHash(),
            doc.GetDocumentVersionCounter())
        {
            Payload = additionalPayload
        };
    }
}