using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Direwolf.Revit.Definitions;

namespace Direwolf.Revit.Utilities;
public static class Helpers
{
    public static void GenerateNewWindow(string title, string content)
    {
        using TaskDialog t = new(title)
        {
            MainContent = content
        };
        t.Show();
    }

    public readonly record struct RevitAppDoc(ExternalCommandData ExternalCommandData)
    {
        public static UIApplication GetApplication(ExternalCommandData cmd) => cmd.Application;
        public static Document GetDocument(ExternalCommandData cmd) => cmd.Application.ActiveUIDocument.Document;
    }

    public static ElementInformation ExtractElementInfo(Element e, Document d)
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

            // is on Workset
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
                ElementIdValue = e.Id.Value,
                ElementUniqueId = e.UniqueId,
                ElementVersionId = e.VersionGuid.ToString(),
                FamilyName = familyName,
                Category = builtInCategory,
                BuiltInCategory = builtInCategory,
                Workset = workset,
                Views = views,
                DesignOption = designOption,
                DocumentOwner = docOwner,
                OwnerViewId = ownerViewId,
                WorksetId = worksetId,
                LevelId = levelId,
                CreatedPhaseId = createdPhaseId,
                DemolishedPhaseId = demolishedPhaseId,
                GroupId = groupId,
                WorkshareId = workshareId,
                IsGrouped = isGrouped,
                IsModifiable = isModifiable,
                IsViewSpecific = isViewSpecific,
                IsBuiltInCategory = isBuiltInCategory,
                IsAnnotative = isAnnotative,
                IsModel = isModel,
                IsPinned = isPinned,
                IsWorkshared = isWorkshared,
                Parameters = null
            };
        }
        else
        {
            throw new ArgumentNullException();
        }
    }
}
