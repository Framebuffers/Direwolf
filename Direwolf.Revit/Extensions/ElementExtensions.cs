using System.Diagnostics;
using Autodesk.Revit.DB;
using Direwolf.Revit.Definitions;
using Direwolf.Revit.Definitions.Primitives;

namespace Direwolf.Revit.Extensions;

public static class ElementExtensions
{
    public static bool TryGetElementEntity(this Element e, 
        RevitDocumentEpisode episode,
        out ElementEntity? elementEntity)
    {
        try
        {
            elementEntity = new ElementEntity(episode, e);
            return true;
        }
        catch (Exception exception)
        {
            _ = exception;
            elementEntity = null;
            return false;
        }
    }

    public static bool TryGetCategory(this Element? e, 
        out Category? category, 
        out BuiltInCategory builtInCategory)
    {
        try
        {
            if (e is not null)
            {
                if (e.Category is null)
                {
                    category = null;
                    builtInCategory = BuiltInCategory.INVALID;
                    return false;
                }

                category = e.Category;
                builtInCategory = e.Category.BuiltInCategory;
                return true;
            }

            category = null;
            builtInCategory = BuiltInCategory.INVALID;
            return false;
        }
        catch (Exception exception)
        {
            Debug.Print(exception.Message);
            throw;
        }
    }

    public static IEnumerable<ParameterEntity> GetAllParameterEntities(this Element e)
    {
        foreach (var (property, type, name) in from orderedProperty in e.GetOrderedParameters()
                 let p = orderedProperty
                 let t = orderedProperty.StorageType.ToString()
                 let n = orderedProperty.Definition.Name
                 select (p, t, n))
        {
            yield return new ParameterEntity
            {
                ParentElementId = Guid.TryParse(e.UniqueId, out var parentElementId) ? parentElementId : Guid.Empty,
                ParameterType = type,
                ParameterName = name,
                ParameterValue = GetValue(property)
            };
        }

        yield break;

        object? GetValue(Parameter p)
        {
            return p.StorageType switch
            {
                StorageType.Integer => p.AsInteger(),
                StorageType.String => p.AsString(),
                StorageType.Double => p.AsDouble(),
                StorageType.ElementId => p.AsElementId().Value,
                _ => null
            };
        }
    }
}