using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Revit.ElementFilters;
using Direwolf.Revit.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Howls
{
    /// <summary>
    /// (very) rudimentary test Howl for all Extension methods.
    /// </summary>
    public record class GetExtensionTest : RevitHowl
    {
        public GetExtensionTest(Autodesk.Revit.DB.Document doc) => SetRevitDocument(doc);

        public override bool Execute()
        {
            var doc = GetRevitDocument();
            string lastName = string.Empty;

            //try
            //{
            IEnumerable<Element?> annotativeElements = doc.GetAnnotativeElements();
            lastName = "getAnnotativeElements";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, annotativeElements.Count().ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //return false;
            //}
            //try
            //{
            IEnumerable<Element?> designOptions = doc.GetDesignOptions();
            lastName = "getDesignOptions";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, designOptions.Count().ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //    SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //    return false;
            //}
            //try
            //{
            IEnumerable<Element?> detailGroups = doc.GetDetailGroups();
            lastName = "getDetailGroups";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, detailGroups.Count().ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //    SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //    return false;
            //}
            //try
            //{
            Dictionary<string, int> elementsByWorkset = doc.GetElementsByWorkset();
            lastName = "getElementsByWorkset";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, elementsByWorkset.Count.ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //    SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //    return false;
            //}
            //try
            //{
            IEnumerable<FailureMessage> errorsAndWarnings = doc.GetErrorsAndWarnings();
            lastName = "getErrorsAndWarnings";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, errorsAndWarnings.Count().ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //    SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //    return false;
            //}
            //try
            //{
            int familyCount = doc.GetFamilies().Count();
            lastName = "getHowManyFamilies";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, familyCount.ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //    SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //    return false;
            //}
            //try
            //{
            int gridLineCount = doc.GetGridLineCount();
            lastName = "getGridLineCount";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, gridLineCount.ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //    SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //    return false;
            //}
            //try
            //{
            IEnumerable<Element?> inPlaceFamilies = doc.GetInPlaceFamilies();
            lastName = "getInPlaceFamilies";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, inPlaceFamilies.Count().ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //    SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //    return false;
            //}
            //try
            //{
            IEnumerable<Family> familiesWithMostTypes = doc.GetFamliesWithMostTypes();
            lastName = "getFamliesWithMostTypes";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, familiesWithMostTypes.Count().ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //    SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //    return false;
            //}
            //try
            //{
            int levelCount = doc.GetLevelCount();
            lastName = "getLevelCount";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, levelCount.ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //    SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //    return false;
            //}
            //try
            //{
            IEnumerable<Element?> mirroredObjects = doc.GetMirroredObjects();
            lastName = "getMirroredObjects";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, mirroredObjects.Count().ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //    SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //    return false;
            //}
            //try
            //{
            IEnumerable<Element?> modelGroups = doc.GetModelGroups();
            lastName = "getModelGroups";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, modelGroups.Count().ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //    SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //    return false;
            //}
            //try
            //{
            IEnumerable<Element?> nonNativeObjectStyles = doc.GetNonNativeObjectStyles();
            lastName = "getNonNativeObjectStyles";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, nonNativeObjectStyles.Count().ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //    SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //    return false;
            //}
            //try
            //{
            IEnumerable<Element?> externalFiles = doc.GetExternalFileReferences();
            lastName = "getImportedInstances";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, externalFiles.Count().ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //    SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //    return false;
            //}
            //try
            //{
            IEnumerable<Element?> unconnectedDucts = doc.GetUnconnectedDucts();
            lastName = "getUnconnectedDucts";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, unconnectedDucts.Count().ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //    SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //    return false;
            //}
            //try
            //{
            IEnumerable<Element?> unconnectedElectrical = doc.GetUnconnectedElectrical();
            lastName = "getUnconnectedElectrical";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, unconnectedElectrical.Count().ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //    SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //    return false;
            //}
            //try
            //{
            IEnumerable<Element?> unconnectedPipes = doc.GetUnconnectedPipes();
            lastName = "getUnconnectedPipes";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, unconnectedPipes.Count().ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //    SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //    return false;
            //}
            //try
            //{
            IEnumerable<Element?> unenclosedRooms = doc.GetUnenclosedRooms();
            lastName = "getUnenclosedRooms";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, unenclosedRooms.Count().ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //    SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //    return false;
            //}
            //try
            //{
            IEnumerable<Element?> unusedFamilies = doc.GetUnusedFamilies();
            lastName = "getUnusedFamilies";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, unusedFamilies.Count().ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //    SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //    return false;
            //}
            //try
            //{

            IEnumerable<Element?> views = doc.GetViews();
            lastName = "getViews";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, views.Count().ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //    SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //    return false;
            //}
            //try
            //{
            SortedDictionary<int, Family> instancesPerFamily = doc.GetInstancesPerFamily();
            lastName = "getInstancesPerFamily";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, instancesPerFamily.Count.ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //    SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //    return false;
            //}
            //try
            //{
            SortedDictionary<Category, List<Family>> familiesByCategory = doc.GetFamiliesByCategory();
            lastName = "getFamiliesByCategory";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, familiesByCategory.Count.ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //    SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //    return false;
            //}
            //try
            //{
            IEnumerable<Element?>? viewsNotInSheets = doc.GetViewsNotInSheets();
            lastName = "getViewsNotInSheets";
            SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, viewsNotInSheets.Count().ToString())));
            Debug.Print(lastName);
            //}
            //catch (Exception e)
            //{
            //    SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            //    return false;
            //}
            return true;
        }

    }
}
