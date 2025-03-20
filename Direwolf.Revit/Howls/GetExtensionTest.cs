using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Revit.ElementFilters;
using Direwolf.Revit.Utilities;
using System;
using System.Collections.Generic;
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

            try
            {
                IEnumerable<Element> annotativeElements = doc.GetAnnotativeElements();
                lastName = "getAnnotativeElements";
                SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, annotativeElements.Count().ToString())));
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
                return false;
            }

            try
            {

                IEnumerable<Element> designOptions = doc.GetDesignOptions();
                lastName = "getDesignOptions";
                SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, designOptions.Count().ToString())));
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
                return false;
            }
            try
            {


                IEnumerable<Element> detailGroups = doc.GetDetailGroups();
                lastName = "getDetailGroups";
                SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, detailGroups.Count().ToString())));
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
                return false;
            }

            try
            {

                Dictionary<string, int> elementsByWorkset = doc.GetElementsByWorkset();
                lastName = "getElementsByWorkset";
                SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, elementsByWorkset.Count.ToString())));
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
                return false;
            }
            try
            {


                IEnumerable<FailureMessage> errorsAndWarnings = doc.GetErrorsAndWarnings();
                lastName = "getErrorsAndWarnings";
                SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, errorsAndWarnings.Count().ToString())));
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
                return false;
            }

            try
            {

                int familyCount = doc.GetFamilies();
                lastName = "getHowManyFamilies";
                SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, familyCount.ToString())));
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
                return false;
            }
            try
            {


                Dictionary<Family, int> familiesWithMostInstances = doc.GetFamiliesWithMostInstances();
                lastName = "getFamiliesWithMostInstances";
                SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, familiesWithMostInstances.Count.ToString())));
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
                return false;
            }

            try
            {

                int gridLineCount = doc.GetGridLineCount();
                lastName = "getGridLineCount";
                SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, gridLineCount.ToString())));
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
                return false;
            }

            try
            {

                IList<Element> importedImages = doc.GetImportedImages();
                lastName = "getImportedImages";
                SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, importedImages.Count.ToString())));
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
                return false;
            }

            try
            {

                IEnumerable<Element> inPlaceFamilies = doc.GetInPlaceFamilies();
                lastName = "getInPlaceFamilies";
                SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, inPlaceFamilies.Count().ToString())));
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
                return false;
            }
            try
            {


                IEnumerable<Family> familiesWithMostTypes = doc.GetFamliesWithMostTypes();
                lastName = "getFamliesWithMostTypes";
                SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, familiesWithMostTypes.Count().ToString())));
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
                return false;
            }
            try
            {


                int levelCount = doc.GetLevelCount();
                lastName = "getLevelCount";
                SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, levelCount.ToString())));
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
                return false;
            }
            try
            {


                IEnumerable<Element> mirroredObjects = doc.GetMirroredObjects();
                lastName = "getMirroredObjects";
                SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, mirroredObjects.Count().ToString())));
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
                return false;
            }

            try
            {

                IEnumerable<Element> modelGroups = doc.GetModelGroups();
                lastName = "getModelGroups";
                SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, modelGroups.Count().ToString())));
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
                return false;
            }
            try
            {


                IEnumerable<Element> nonNativeObjectStyles = doc.GetNonNativeObjectStyles();
                lastName = "getNonNativeObjectStyles";
                SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, nonNativeObjectStyles.Count().ToString())));
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
                return false;
            }

            try
            {

                IEnumerable<Element> importedInstances = doc.GetImportedInstances();
                lastName = "getImportedInstances";
                SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, importedInstances.Count().ToString())));
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
                return false;
            }
            try
            {


                IEnumerable<Element> unconnectedDucts = doc.GetUnconnectedDucts();
                lastName = "getUnconnectedDucts";
                SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, unconnectedDucts.Count().ToString())));
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
                return false;
            }
            try
            {


                IEnumerable<Element> unconnectedElectrical = doc.GetUnconnectedElectrical();
                lastName = "getUnconnectedElectrical";
                SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, unconnectedElectrical.Count().ToString())));
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
                return false;
            }

            try
            {

                IEnumerable<Element> unconnectedPipes = doc.GetUnconnectedPipes();
                lastName = "getUnconnectedPipes";
                SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, unconnectedPipes.Count().ToString())));
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
                return false;
            }
            try
            {


                IEnumerable<Element> unenclosedRooms = doc.GetUnenclosedRooms();
                lastName = "getUnenclosedRooms";
                SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, unenclosedRooms.Count().ToString())));
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
                return false;
            }

            try
            {

                IEnumerable<Element> unusedFamilies = doc.GetUnusedFamilies();
                lastName = "getUnusedFamilies";
                SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, unusedFamilies.Count().ToString())));
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
                return false;
            }

            try
            {

                IEnumerable<Element> views = doc.GetViews();
                lastName = "getViews";
                SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, views.Count().ToString())));
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
                return false;
            }

            return true;
        }

    }
}
