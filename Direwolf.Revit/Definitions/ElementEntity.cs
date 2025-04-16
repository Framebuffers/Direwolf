using Autodesk.Revit.DB;
using Direwolf.Revit.Definitions.Primitives;
using Direwolf.Revit.Extensions;

namespace Direwolf.Revit.Definitions;

public readonly record struct ElementEntity
{
    public ElementEntity(RevitDocumentEpisode episode, Element e)
    {
        e.TryGetCategory(out var category, out var builtInCategory);
        
        DocumentEpisode = episode;
        ElementIdValue = e.Id.Value;
        ElementUniqueIdValue = Guid.TryParse(
            e.UniqueId, out var parentElementId)
            ? parentElementId
            : Guid.Empty;
        ElementBuiltInCategory = builtInCategory.ToString();
        ElementCategory = category?.Name ?? null;
        ElementType = e.GetType().FullName ?? null;
        ElementCategoryType = category?.CategoryType.ToString() ?? null;
        Parameters = e.GetAllParameterEntities();
    }
    public RevitDocumentEpisode DocumentEpisode { get; init; } 
    public double ElementIdValue { get; init; }
    public Guid ElementUniqueIdValue { get; init; }
    public string? ElementBuiltInCategory { get; init; }
    public string? ElementCategory { get; init; } = null;
    public string? ElementType { get; init; }
    public string? ElementCategoryType { get; init; }
    public IEnumerable<ParameterEntity>? Parameters { get; init; }
}

public readonly record struct ParameterEntity
{
    public Guid? ParentElementId { get; init; }
    public string? ParameterType { get; init; }
    public string? ParameterName { get; init; }
    public object? ParameterValue { get; init; }
}

