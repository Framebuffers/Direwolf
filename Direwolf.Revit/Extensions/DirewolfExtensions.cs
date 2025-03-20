using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Direwolf.Revit.Definitions;

namespace Direwolf.Revit.Utilities;
public static class DirewolfExtensions
{
    public static ICollection<Element>? GetAllValidElements(this Document doc)
    {
        return new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .WhereElementIsViewIndependent()
                .ToElements();
    }

    public static Dictionary<string, object> ExtractParameterMap(this Element e)
    {
        Dictionary<string, object> results = [];
        ParameterSet ps = e.Parameters;
        foreach (Autodesk.Revit.DB.Parameter p in ps)
        {
            string GetValue()
            {
                return p.StorageType switch
                {
                    StorageType.None => "None",
                    StorageType.Integer => p.AsInteger().ToString(),
                    StorageType.Double => p.AsDouble().ToString(),
                    StorageType.String => p.AsString(),
                    StorageType.ElementId => p.AsElementId().ToString(),
                    _ => "None",
                };
            }

            Dictionary<string, string> data = new()
            {
                ["GUID"] = p.GUID.ToString(),
                ["Type"] = p.GetTypeId().TypeId,
                ["HasValue"] = p.HasValue.ToString(),
                ["Value"] = GetValue()

            };
            results.Add(p.Definition.Name, results);
        }
        return results;
    }

    public static Dictionary<string, object> ExtractElementData(this Element element) => new(new Dictionary<string, object>
    {
        [element.Id.ToString()] = new Dictionary<string, object>()
        {
            ["UniqueId"] = element.UniqueId ?? 0.ToString(),
            ["VersionGuid"] = element.VersionGuid.ToString(),
            ["isPinned"] = element.Pinned.ToString(),
            ["Data"] = ExtractParameterMap(element)
        }
    });

    public static ElementInformation GetElementInformation(this Element e, Document d)
    {
        if (e is not null && e.IsValidObject && e.Category is not null && e.Category.CategoryType is not CategoryType.Invalid || e?.Category?.CategoryType is not CategoryType.Internal)
        {
            string? familyName = string.Empty;
            string? category = string.Empty;
            string? builtInCategory = string.Empty;
            string? workset = string.Empty;
            string[]? views = [];
            string? designOption = string.Empty;
            string? docOwner = string.Empty;
            string? ownerViewId = string.Empty;
            string? worksetId = string.Empty;
            string? createdPhaseId = string.Empty;
            string? demolishedPhaseId = string.Empty;
            string? groupId = string.Empty;
            string? workshareId = string.Empty;
            string levelId = string.Empty;
            bool? isGrouped = false;
            bool? isModifiable = false;
            bool? isViewSpecific = false;
            bool? isBuiltInCategory = false;
            bool? isAnnotative = false;
            bool? isModel = false;
            bool? isPinned = false;
            bool? isWorkshared = false;


            FamilyInstance? fm = e as FamilyInstance;

            // isGrouped
            if (e?.GroupId is not null)
            {
                isGrouped = true;
                groupId = e.GroupId.ToString();
            }

            // isModifiable
            if (e.IsModifiable) isModifiable = true;

            // isViewSpecific
            if (!e.ViewSpecific)
            {
                familyName = fm?.Symbol.Family.Name;
            }
            else
            {
                isViewSpecific = true;
                ownerViewId = e.OwnerViewId.ToString();
            }

            // isBuiltInCategory + builtInCategory
            if (e.Category is not null && e.Category.BuiltInCategory is not BuiltInCategory.INVALID)
            {
                isBuiltInCategory = true;
                category = e.Category.Name;
            }

            // is on workset
            if (e.WorksetId is not null) worksetId = e.WorksetId.ToString();

            // phase info
            if (e.HasPhases())
            {
                if (e.CreatedPhaseId is not null) createdPhaseId = e.CreatedPhaseId.ToString();
                if (e.DemolishedPhaseId is not null) demolishedPhaseId = e.DemolishedPhaseId.ToString();
            }

            // design option info
            if (e.DesignOption is not null) designOption = e.DesignOption.Name;

            // document version
            if (e.Document is not null) docOwner = d.CreationGUID.ToString();

            // pinned
            isPinned = e.Pinned;

            // worksharing
            if (d.IsWorkshared)
            {
                isWorkshared = true;
                workshareId = d.WorksharingCentralGUID.ToString();
            }


            // level where it is located
            if (e?.LevelId is not null) levelId = e.LevelId.ToString();


            return new ElementInformation
            {
                idValue = e.Id.Value,
                uniqueElementId = e.UniqueId,
                elementVersionId = e.VersionGuid.ToString(),
                familyName = familyName,
                category = builtInCategory,
                builtInCategory = builtInCategory,
                workset = workset,
                views = views,
                designOption = designOption,
                documentOwner = docOwner,
                ownerViewId = ownerViewId,
                worksetId = worksetId,
                levelId = levelId,
                createdPhaseId = createdPhaseId,
                demolishedPhaseId = demolishedPhaseId,
                groupId = groupId,
                workshareId = workshareId,
                isGrouped = isGrouped,
                isModifiable = isModifiable,
                isViewSpecific = isViewSpecific,
                isBuiltInCategory = isBuiltInCategory,
                isAnnotative = isAnnotative,
                isModel = isModel,
                isPinned = isPinned,
                isWorkshared = isWorkshared,
                Parameters = null
            };
        }
        else
        {
            throw new ArgumentNullException($"Could not extract information. Document {d.Title} is invalid, or does not have information about element {e.Name}::{e.Id.Value}");
        }
    }
}
