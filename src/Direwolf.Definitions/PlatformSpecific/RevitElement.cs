using System.Runtime.Caching;
using System.Text.Json.Serialization;
using Autodesk.Revit.DB;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.LLM;
using Direwolf.Definitions.PlatformSpecific.Extensions;
using Direwolf.Definitions.PlatformSpecific.Serialization;
using Direwolf.Definitions.Serialization;

namespace Direwolf.Definitions.PlatformSpecific;

/// <summary>
///     A symbolic representation of a <see cref="Autodesk.Revit.DB.Element" /> inside the context of
///     <see cref="Direwolf" />.
///     It identifies, defines and specifies the state of any valid <see cref="Autodesk.Revit.DB.Element" /> inside the
///     Revit <see cref="Document" /> at any given time.
///     The purpose of it is to have a lighter data structure that mirrors the state of the currently-loaded Revit
///     <see cref="Document" />, that can be cached and
///     retrieved faster than doing a normal Revit API query.
/// </summary>
/// <param name="Id">A Collision-Resistant Unique Identifier</param>
/// <param name="CategoryType">Category JsonType: Annotative, Model, Internal, AnalyticalModel, Invalid</param>
/// <param name="BuiltInCategory">BuiltInCategory Enum</param>
/// <param name="ElementTypeId">ElementId of the type corresponding to the Element being defined.</param>
/// <param name="ElementUniqueId">The UniqueId of this Element</param>
/// <param name="ElementId">The ElementId value of this Element</param>
/// <param name="ElementName">The human-readable name of this Element, if applicable.</param>
/// <param name="Parameters">A list of all the Params held inside this Element.</param>
[JsonConverter(typeof(ElementJsonSerializer))]
public readonly record struct RevitElement(
    Cuid Id,
    CategoryType CategoryType,
    BuiltInCategory BuiltInCategory,
    ElementId? ElementTypeId,
    string ElementUniqueId,
    ElementId? ElementId,
    string? ElementName,
    IReadOnlyList<RevitParameter?> Parameters)
{
    /// <summary>
    ///     Creates a new record of the given <see cref="ElementId" />, held inside the given <see cref="Document" />
    /// </summary>
    /// <param name="doc">Revit Document</param>
    /// <param name="elementUniqueId"></param>
    /// <returns>
    ///     A record containing identification, description and details of a <see cref="Autodesk.Revit.DB.Element" />
    /// </returns>
    public static RevitElement? Create(Document doc,
        string elementUniqueId)
    {
        var members = elementUniqueId.DeconstructElementUniqueId(doc);
        if (members.Element is null)
            return null;
        members.Element.TryGetParameters(out var param);
        var category = GetElementCategory(members.Element);
        return new RevitElement(doc.CreateRevitId(),
            category.CategoryType,
            category.BuiltInCategory,
            members.ElementTypeId,
            elementUniqueId,
            members.ElementId,
            members.Element.Name ?? string.Empty,
            param);
    }

    /// <summary>
    ///     Create a <see cref="RevitElement" /> with only certain <see cref="BuiltInParameter" /> loaded inside
    ///     <see cref="RevitElement.Parameters" />
    /// </summary>
    /// <param name="doc">Revit Document</param>
    /// <param name="elementUniqueId">
    ///     The Element's Unique Identifier found on
    ///     <see cref="Autodesk.Revit.DB.Element.UniqueId" />
    /// </param>
    /// <param name="parameters">The <see cref="BuiltInParameter" /> to check the values for.</param>
    /// <returns>
    ///     A <see cref="RevitElement" /> where the <see cref="RevitElement.Parameters" /> property only contains
    ///     <see cref="RevitParameter" /> for the specified <see cref="BuiltInParameter" />
    /// </returns>
    public static RevitElement? CreateWithSpecificParameters(Document doc,
        string elementUniqueId,
        BuiltInParameter[] parameters)
    {
        var members = elementUniqueId.DeconstructElementUniqueId(doc);
        if (members.Element is null)
            return null;
        members.Element.TryGetParameters(out var param);
        var category = GetElementCategory(members.Element);

        return new RevitElement(
            doc.CreateRevitId(),
            category.CategoryType,
            category.BuiltInCategory,
            members.ElementTypeId,
            members.Element.UniqueId,
            members.ElementId,
            members.Element.Name ?? string.Empty,
            param);
    }

    /// <summary>
    ///     Creates a record of the given <see cref="Autodesk.Revit.DB.Element" />, held inside the given
    ///     <see cref="Document" />, as a
    ///     <see cref="CacheItem" /> to be used inside the <see cref="ObjectCache" /> inside <see cref="Direwolf" />.
    /// </summary>
    /// <remarks>
    ///     Inside <see cref="Direwolf" />, the key for all <see cref="CacheItem" /> held inside the <see cref="ObjectCache" />
    ///     is <see cref="Autodesk.Revit.DB.Element.UniqueId" />
    /// </remarks>
    /// <param name="doc">Revit Document</param>
    /// <param name="elementUniqueId">
    ///     The Element's Unique Identifier found on
    ///     <see cref="Autodesk.Revit.DB.Element.UniqueId" />
    /// </param>
    /// <param name="rvt">The <see cref="RevitElement" /> generated.</param>
    /// <returns></returns>
    public static CacheItem? CreateAsCacheItem(Document doc,
        string elementUniqueId,
        out RevitElement? rvt)
    {
        var members = elementUniqueId.DeconstructElementUniqueId(doc);
        if (members.Element is null)
        {
            rvt = null;
            return null;
        }

        members.Element.TryGetParameters(out var param);
        var category = GetElementCategory(members.Element);
        var newElement = new RevitElement(
            doc.CreateRevitId(),
            category.CategoryType,
            category.BuiltInCategory,
            members.ElementTypeId,
            members.Element.UniqueId,
            members.ElementId,
            members.Element.Name ?? string.Empty,
            param);
        rvt = newElement;
        return new CacheItem(newElement.Id.ToString(), newElement);
    }

    public CacheItem AsCacheItem()
    {
        return new CacheItem(ElementUniqueId, this);
    }

    /// <summary>
    ///     Concatenates the <see cref="Cuid.Counter" /> and <see cref="Cuid.Fingerprint" /> portions of
    ///     a Direwolf-generated CUID. These IDs are based on a truncated Base36 hash of both the
    ///     <see cref="Document.CreationGUID" />
    ///     and Document.GetDocumentVersion(doc).NumberOfSaves of the given <see cref="Document" />
    /// </summary>
    /// <remarks>
    ///     Each <see cref="RevitElement.Id" /> is created using <see cref="CuidDriver.NewDirewolfId" /> requestType.
    ///     This requestType uses the <see cref="Cuid.Counter" /> and <see cref="Cuid.Fingerprint" /> to
    ///     store the number of saves and the <see cref="Document.CreationGUID" /> respectively, deviating a bit
    ///     from a standard CUID.
    ///     The way <see cref="Direwolf" /> indexes <see cref="RevitElement" /> inside the <see cref="ObjectCache" /> is
    ///     through
    ///     this concatenated hash, identifying each "Episode" (a given Document at a given save state), at a given time
    ///     (using <see cref="Cuid.TimestampNumeric" />)
    ///     Therefore, it is safe to use these properties as a way to separate two
    ///     <see cref="Autodesk.Revit.DB.Element.UniqueId" />
    ///     when more than one snapshot is cached.
    ///     This allows <see cref="Direwolf" /> to track the history of a single element's changes over time.
    /// </remarks>
    /// <param name="r">RevitElement to extract the Document ID from.</param>
    /// <returns>
    ///     A string with the concatenated Base46 hash of the CreationGUID and SaveCount of the <see cref="Document" />
    /// </returns>
    private static string GetEmbeddedDocumentId(RevitElement r)
    {
        return string.Concat(r.Id.Counter,
            r.Id.Fingerprint);
    }

    /// <summary>
    ///     Checks if a given <see cref="RevitElement.Id" /> has the same <see cref="Cuid.Counter" /> and
    ///     <see cref="Cuid.Fingerprint" />
    ///     as one generated by hashing the <see cref="Document" />'s <see cref="Document.CreationGUID" /> and its SaveCount.
    /// </summary>
    /// <param name="r">A <see cref="RevitElement" />.</param>
    /// <param name="doc">The Revit <see cref="Document" /> to check against.</param>
    /// <returns>True if the <see cref="RevitElement" /> corresponds to the given <see cref="Document" />, False otherwise.</returns>
    public static bool BelongsToDocument(RevitElement r,
        Document doc)
    {
        return CuidExtensions.GetDocumentUuidHash(doc)
            .Equals(GetEmbeddedDocumentId(r));
    }

    // /// <summary>
    // /// Aggregate <see cref="RevitElement"/> into a single Wolfpack with RevitElements, where the <see cref="WolfpackMessage.Parameters"/> dictionary sorts them by <see cref="RevitElement.ElementUniqueId"/>
    // /// as key, and <see cref="RevitElement"/> itself as value.
    // /// </summary>
    // /// <param name="elements"></param>
    // /// <param name="wolfpack"></param>
    // /// <param name="updated"></param>
    // /// <returns></returns>
    // public static MessageResponse Aggregate(in RevitElement[] elements, WolfpackMessage wolfpack,
    //     out WolfpackMessage updated)
    // {
    //     var uniqueKeys = wolfpack.Parameters;
    //
    //     if (uniqueKeys is null)
    //     {
    //         updated = wolfpack;
    //         return MessageResponse.Result;
    //     }
    //
    //     foreach (var (key, result) in from incomingElement in elements
    //              let newKey = incomingElement.ElementUniqueId
    //              where wolfpack.Parameters.ContainsKey(newKey) is false
    //              select (newKey, incomingElement))
    //     {
    //
    //         uniqueKeys.Add(key, result);
    //     }
    //     
    //     updated = wolfpack;
    //     return MessageResponse.Result;
    // }

    /// <summary>
    ///     Checks if an <see cref="Autodesk.Revit.DB.Element" /> has a valid <see cref="Autodesk.Revit.DB.Category" />,
    ///     and returns its corresponding values.
    /// </summary>
    /// <param name="e">An <see cref="Autodesk.Revit.DB.Element" /> to check its category for.</param>
    /// <returns>
    ///     A tuple with the <see cref="CategoryType" /> and <see cref="BuiltInCategory" /> held inside the
    ///     <see cref="Autodesk.Revit.DB.Category" /> property.
    /// </returns>
    private static (CategoryType CategoryType, BuiltInCategory BuiltInCategory) GetElementCategory(Element e)
    {
        var category = e.Category;
        return category is null
            ? (CategoryType.Invalid, BuiltInCategory.INVALID)
            : (category.CategoryType, category.BuiltInCategory);
    }
}