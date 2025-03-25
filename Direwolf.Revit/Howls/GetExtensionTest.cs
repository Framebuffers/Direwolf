using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Revit.Extensions;
using System.Diagnostics;

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
                //IEnumerable<Element?> annotativeElements = doc.GetAllAnnotativeElemens();
                //lastName = "getAnnotativeElements";
               //SendCatchToCallback(new Prey(lastName, annotativeElements.Select(x => x.GetParametersFromElement())));
                //Debug.Print(lastName + " " + annotativeElements.Count().ToString());
                
                //IEnumerable<Element?> designOptions = doc._GetDesignOptions();
                //lastName = "getDesignOptions";
                ////SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, designOptions.Count().ToString())));
                //Debug.Print(lastName + " " + designOptions.Count().ToString());
                
                //IEnumerable<Element?> detailGroups = doc.GetAllDetailGroupsInDocument();
                //lastName = "getDetailGroups";
                ////SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, detailGroups.Count().ToString())));
                //Debug.Print(lastName + " " + detailGroups.Count().ToString());
                
                //Dictionary<string, int> elementsByWorkset = doc._GetElementsByWorkset();
                //lastName = "getElementsByWorkset";
                ////SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, elementsByWorkset.Count.ToString())));
                //Debug.Print(lastName + "" + elementsByWorkset.Count().ToString());
                
                //IEnumerable<FailureMessage> errorsAndWarnings = doc._GetErrorsAndWarnings();
                //lastName = "getErrorsAndWarnings";
                ////SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, errorsAndWarnings.Count().ToString())));
                //Debug.Print(lastName + "" + errorsAndWarnings.Count().ToString());
                
                //int familyCount = doc._GetFamilies().Count();
                //lastName = "getHowManyFamilies";
                ////SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, familyCount.ToString())));
                //Debug.Print(lastName + "" + familyCount.ToString());
                
                //int gridLineCount = doc.GetGridLineCount();
                //lastName = "getGridLineCount";
                ////SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, gridLineCount.ToString())));
                //Debug.Print(lastName + " " + gridLineCount.ToString());
                
                //IEnumerable<Element?> inPlaceFamilies = doc._GetInPlaceFamilies();
                //lastName = "getInPlaceFamilies";
                ////SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, inPlaceFamilies.Count().ToString())));
                //Debug.Print(lastName + " " + inPlaceFamilies.Count().ToString());
                
                //IEnumerable<Family> familiesWithMostTypes = doc._GetFamliesWithMostTypes();
                //lastName = "getFamliesWithMostTypes";
                ////SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, familiesWithMostTypes.Count().ToString())));
                //Debug.Print(lastName);

                //int levelCount = doc._GetLevelCount();
                //lastName = "getLevelCount";
                ////SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, levelCount.ToString())));
                //Debug.Print(lastName);
                
                //IEnumerable<Element?> mirroredObjects = doc._GetMirroredObjects();
                //lastName = "getMirroredObjects";
                ////SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, mirroredObjects.Count().ToString())));
                //Debug.Print(lastName);
                
                //IEnumerable<Element?> modelGroups = doc._GetModelGroups();
                //lastName = "getModelGroups";
                ////SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, modelGroups.Count().ToString())));
                //Debug.Print(lastName);
  
                //IEnumerable<Element?> nonNativeObjectStyles = doc._GetNonNativeObjectStyles();
                //lastName = "getNonNativeObjectStyles";
                ////SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, nonNativeObjectStyles.Count().ToString())));
                //Debug.Print(lastName);
      
                //IEnumerable<Element?> externalFiles = doc._GetExternalFileReferences();
                //lastName = "getImportedInstances";
                ////SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, externalFiles.Count().ToString())));
                //Debug.Print(lastName);

                //IEnumerable<Element?> unconnectedDucts = doc._GetUnconnectedDucts();
                //lastName = "getUnconnectedDucts";
                ////SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, unconnectedDucts.Count().ToString())));
                //Debug.Print(lastName);

                //IEnumerable<Element?> unconnectedElectrical = doc._GetUnconnectedElectrical();
                //lastName = "getUnconnectedElectrical";
                ////SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, unconnectedElectrical.Count().ToString())));
                //Debug.Print(lastName);

                //IEnumerable<Element?> unconnectedPipes = doc._GetUnconnectedPipes();
                //lastName = "getUnconnectedPipes";
                ////SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, unconnectedPipes.Count().ToString())));
                //Debug.Print(lastName);

                //IEnumerable<Element?> unenclosedRooms = doc._GetUnenclosedRooms();
                //lastName = "getUnenclosedRooms";
                ////SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, unenclosedRooms.Count().ToString())));
                //Debug.Print(lastName);
              
                //IEnumerable<Element?> unusedFamilies = doc._GetUnusedFamilies();
                //lastName = "getUnusedFamilies";
                ////SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, unusedFamilies.Count().ToString())));
                //Debug.Print(lastName);

                //IEnumerable<Element?> views = doc._GetViews();
                //lastName = "getViews";
                ////SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, views.Count().ToString())));
                //Debug.Print(lastName);
                
                //Dictionary<string, int> instancesPerFamily = doc._GetInstancesPerFamily();
                //lastName = "getInstancesPerFamily";
                ////SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, instancesPerFamily.Count.ToString())));
                //Debug.Print(lastName);

                //Dictionary<Category, List<Family>> familiesByCategory = doc._GetFamiliesByCategory();
                //lastName = "getFamiliesByCategory";
                ////SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, familiesByCategory.Count.ToString())));
                //Debug.Print(lastName);

                //IEnumerable<Element?>? viewsNotInSheets = doc._GetViewsNotInSheets();
                //lastName = "getViewsNotInSheets";
                ////SendCatchToCallback(new Prey(new KeyValuePair<string, string>(lastName, viewsNotInSheets.Count().ToString())));
                //Debug.Print(lastName);

                Dictionary<string, object> data = [];
                data["getAnnotativeElements"] = doc.GetAllDetailGroupsInDocument();
                //data["getDesignOptions"] = doc._GetDesignOptions().Select(x => x?.GetParametersFromElement());
                //data["getDetailGroups"] = doc.GetAllDetailGroupsInDocument().Select(x => x?.GetParametersFromElement());
                //data["getElementsByWorkset"] = doc._GetElementsByWorkset();
                //data["getErrorsAndWarnings"] = doc._GetErrorsAndWarnings();
                //data["getHowManyFamilies"] = doc._GetFamilies().Count();
                //data["getGridLineCount"] = doc.GetGridLineCount();
                //data["getInPlaceFamilies"] = doc._GetInPlaceFamilies().Select(x => x?.GetParametersFromElement());
                //data["getFamliesWithMostTypes"] = doc._GetFamliesWithMostTypes().Select(x => x?.GetParametersFromElement());
                //data["getLevelCount"] = doc._GetLevelCount();
                //data["getMirroredObjects"] = doc._GetMirroredObjects().Select(x => x?.GetParametersFromElement()); 
                //data["getModelGroups"] = doc._GetModelGroups().Select(x => x?.GetParametersFromElement()); 
                //data["getNonNativeObjectStyles"] = doc._GetNonNativeObjectStyles().Select(x => x?.GetParametersFromElement()); 
                //data["getImportedInstances"] = doc._GetExternalFileReferences().Select(x => x?.GetParametersFromElement()); 
                //data["getUnconnectedDucts"] = doc._GetUnconnectedDucts().Select(x => x? .GetParametersFromElement()); 
                //data["getUnconnectedElectrical"] = doc._GetUnconnectedElectrical().Select(x => x?.GetParametersFromElement()); 
                //data["getUnconnectedPipes"] = doc._GetUnconnectedPipes().Select(x => x?.GetParametersFromElement()); 
                //data["getUnenclosedRooms"] = doc._GetUnenclosedRooms().Select(x => x?.GetParametersFromElement()); 
                //data["getUnusedFamilies"] = doc._GetUnusedFamilies().Select(x => x?.GetParametersFromElement()); 
                //data["getViews"] = doc._GetViews().Select(x => x?.GetParametersFromElement()); 
                //data["getInstancesPerFamily"] = doc._GetInstancesPerFamily();
                //data["getFamiliesByCategory"] = doc._GetFamiliesByCategory();
                //data["getViewsNotInSheets"] = doc._GetViewsNotInSheets().Select(x => x?.GetParametersFromElement()); 

                SendCatchToCallback(new Prey(data));

            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Extension throwed exception at {lastName}: {e.Message}"));
            }
            return true;
        }

    }
}
