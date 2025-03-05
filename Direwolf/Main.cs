using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Revit.Async;
using System.Text.Json;
using System.Diagnostics;
using Direwolf.Examples.Howlers;
using Direwolf.Definitions;

namespace Direwolf
{
    /// <summary>
    /// Benchmark code.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public partial class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            Dictionary<string, Dictionary<string, double>> FinalResults = [];
            double cumulativeNative = 0;

            /*
             * Revit Native Command
             */
            Dictionary<string, double> NativeResults = [];
            Stopwatch s1 = new();
            s1.Start();

            // first test
            WriteToFile("elements_by_id-Revit.json", GetElementIdByFamily_Native(doc));
            s1.Stop();
            cumulativeNative += s1.Elapsed.TotalSeconds;
            NativeResults.Add("Test: Get Element Id by Family - Revit Native Command. Time in seconds.", s1.Elapsed.TotalSeconds);

            // second test
            s1.Restart();
            WriteToFile("family_parameters-Revit.json", GetRevitElementInformation_Native(doc));
            NativeResults.Add("Test: Get all Element Parameters - Revit Native Command. Time in seconds", s1.Elapsed.TotalSeconds);
            s1.Stop();
            cumulativeNative += s1.Elapsed.TotalSeconds;

            // writing result to file
            FinalResults.Add($"NativeRevit = {cumulativeNative}", NativeResults);

            /*
             * Direwolf Command
             */
            Dictionary<string, double> DirewolfResults = [];
            Stopwatch direwolf = new();
            double cumulativeDirewolf = 0;
            direwolf.Start();
            RevitTask.Initialize(commandData.Application);
            Direwolf dw = new();

            // first test
            dw.ExecuteQueryAsync(new RevitElementDispatch(doc), "ElementIdByFamilyInformation");
            direwolf.Stop();
            cumulativeDirewolf += direwolf.Elapsed.TotalSeconds;
            DirewolfResults.Add("Test: Get Element Id by Family - Direwolf Command. Time in seconds.", direwolf.Elapsed.TotalSeconds);
            
            // second test
            direwolf.Restart();
            dw.ExecuteQueryAsync(new RevitParameterDispatch(doc), "ParameterDataInformation");
            direwolf.Stop();
            cumulativeDirewolf += direwolf.Elapsed.TotalSeconds;
            DirewolfResults.Add("Test: Get all Element Parameters - Direwolf Command. Time in seconds.", direwolf.Elapsed.TotalSeconds);

            // writing result to file
            dw.WriteToFile("direwolf.json");
            FinalResults.Add($"Direwolf = {cumulativeDirewolf}", DirewolfResults);

            /*
             * Final results
             */
            FinalResults.Add("Final", new Dictionary<string, double>() { ["TotalTime"] =  (cumulativeNative + cumulativeDirewolf) });
            string serializeResults() => JsonSerializer.Serialize(FinalResults, new JsonSerializerOptions() { WriteIndented = true });
            WriteToFile("final_results.json", serializeResults());
            
            // ui results
            TaskDialog t = new("Results")
            {
                MainContent = serializeResults()
            };
            t.Show();
            return Result.Succeeded;
        }

        public Result Execute_1(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // This is a benchmark of data extraction between a s1 syncronous Revit Command, and a Direwolf asyncronous Command.
            // The one difference from any other Revit.Async command, is that Direwolf has a special data structure.
            // Here's a s1 Revit command:

            Document doc = commandData.Application.ActiveUIDocument.Document;
            Dictionary<string, double> Results = [];

            Stopwatch native = new();
            native.Start();
            // Here's the command that will run in both cases:
            //      It returns a dictionary with all valid elements sorted by family.
            ICollection<Element> allValidElements = new FilteredElementCollector(doc)
                        .WhereElementIsNotElementType()
                        .WhereElementIsViewIndependent()
                        .ToElements();

            var elementsSortedByFamilyNative = new Dictionary<string, List<long>>();
            foreach ((Element e, string familyName) in from Element e in allValidElements // create two variables, Element e and string familyType
                                                       let f = e as FamilyInstance // cast each element as a FamilyInstance
                                                       where f is not null // check if it's not null
                                                       let familyName = f.Symbol.Family.Name // assign the family name to the variable familyName
                                                       select (e, familyName)) // get the variables back
            {
                if (!elementsSortedByFamilyNative.TryGetValue(familyName, out List<long>? value))
                {
                    value = [];
                    elementsSortedByFamilyNative[familyName] = value;
                }
                value.Add(e.Id.Value);
            }
            WriteToFile("elementsByFamily-s1.json", elementsSortedByFamilyNative);

            // Add results from s1 Revit command.
            native.Stop();
            Results.Add("Native Revit Command", native.Elapsed.TotalSeconds);

            // ----------------------------------------------------------------------------------------------------

            // Now, the async Direwolf version!
            Stopwatch direwolf = new();
            direwolf.Start();
            RevitTask.Initialize(commandData.Application);

            // Direwolf will:
            //      - Create a new Howler (in this case, a dispatcher that holds a reference to a Revit document).
            //      - Load a Howls (it's a set of instructions to perform on a given object. In this case, it'll perform the same thing as above.
            //      - Create a Wolf (a worker that reads instructions (the Howls) and has a Callback to its summoner (the Howler).
            //      - When a query is executed, the Howler attaches itself and their respective Howls to each Wolf, and calls the function Run() inside each one.
            //      - The Wolf then can do anything. It can execute the Howls, or anything else. It has a stack to stash all the things it finds (each one called Catch)
            //      - If a Howls, when executed, returns any Catch, it *directly* pipes it back to the Wolf through a callback. It does *not* store any data on its own.
            //      - The Wolf can process any catches when they arrive in the stack. It's in charge of sending Catches up the chain.
            //      - When all executions are done, the Howler returns a Wolfpack: a serializable bundle of information captured in the process.
            //      - This Wolfpack can be stored as a JSON object, manipulated further, or anything. It's just a JSON!
            // TL;DR:
            //  A Howler attaches Howls to a pack of Wolves, each one with a different mission.
            //  The instruction they're given mines the data, and pipes it back to the Wolves.
            //  Each Wolf can process it, or send it directly to the Howler. Then it just becomes a JSON.
            //
            //  Howler <-> Wolf -> Howls <-> Endpoint
            //             + Catches<-|
            Direwolf dw = new();
            dw.ExecuteQueryAsync(new RevitElementDispatch(doc), "DocumentInformation");

            // Add results from Direwolf
            direwolf.Stop();
            Results.Add("Direwolf Revit Command", direwolf.Elapsed.TotalSeconds);

            // Show final results
            JsonSerializerOptions opt = new()
            {
                WriteIndented = true
            };

            TaskDialog t = new("NativeResults")
            {
                MainContent = JsonSerializer.Serialize(Results, opt)
            };
            t.Show();
            return Result.Succeeded;
        }

        public static void WriteToFile(string filename, object obj)
        {
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), filename);
            File.WriteAllText(fileName, JsonSerializer.Serialize(obj));
        }

        private static Dictionary<string, object> ProcessParameterMap(Element element)
        {
            try
            {
                Dictionary<string, object> results = [];
                ParameterSet ps = element.Parameters;
                foreach (Parameter p in ps)
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
            catch
            {
                return [];
            }
        }

        private static Dictionary<string, object> ExtractElementData(Element element) => new(new Dictionary<string, object>
        {
            [element.Id.ToString()] = new Dictionary<string, object>()
            {
                ["UniqueId"] = element.UniqueId ?? 0.ToString(),
                ["VersionGuid"] = element.VersionGuid.ToString(),
                ["IsPinned"] = element.Pinned.ToString(),
                ["Data"] = ProcessParameterMap(element)
            }
        });

        public static Dictionary<string, object> GetElementIdByFamily_Native(Document RevitDocument)
        {
            ICollection<Element> allValidElements = new FilteredElementCollector(RevitDocument)
                        .WhereElementIsNotElementType()
                        .WhereElementIsViewIndependent()
                        .ToElements();

            var elementsSortedByFamilyNative = new Dictionary<string, List<long>>();
            foreach ((Element e, string familyName) in from Element e in allValidElements // create two variables, Element e and string familyType
                                                       let f = e as FamilyInstance // cast each element as a FamilyInstance
                                                       where f is not null // check if it's not null
                                                       let familyName = f.Symbol.Family.Name // assign the family name to the variable familyName
                                                       select (e, familyName)) // get the variables back
            {
                if (!elementsSortedByFamilyNative.TryGetValue(familyName, out List<long>? value))
                {
                    value = [];
                    elementsSortedByFamilyNative[familyName] = value;
                }
                value.Add(e.Id.Value);
            }
            return new Dictionary<string, object>()
            {
                ["ElementsByFamily"] = elementsSortedByFamilyNative
            };
        }

        public static Dictionary<string, object> GetRevitElementInformation_Native(Document RevitDocument)
        {
            try
            {
                Dictionary<string, List<Dictionary<string, object>>> Catches = [];
                ICollection<Element> allValidElements = new FilteredElementCollector(RevitDocument)
                    .WhereElementIsNotElementType()
                    .WhereElementIsViewIndependent()
                    .ToElements();
                Dictionary<string, List<Element>> elementsSortedByFamily = [];

                foreach ((Element e, string familyName) in from Element e in allValidElements
                                                           let f = e as FamilyInstance
                                                           where f is not null
                                                           let familyName = f.Symbol.Family.Name
                                                           select (e, familyName))
                {
                    if (!elementsSortedByFamily.TryGetValue(familyName, out List<Element>? value))
                    {
                        value = new List<Element>();
                        elementsSortedByFamily[familyName] = value;
                    }
                    value.Add(e);
                }

                foreach (KeyValuePair<string, List<Element>> family in elementsSortedByFamily)
                {
                    List<Dictionary<string, object>> elementData = [];
                    elementData.AddRange(family.Value.Select(ExtractElementData));

                    if (Catches.TryGetValue(family.Key, out List<Dictionary<string, object>>? existingElementData))
                    {
                        existingElementData.AddRange(elementData);
                    }
                    else
                    {
                        Catches[family.Key] = elementData;
                    }
                }

                return new Dictionary<string, object>(new Dictionary<string, object>()
                {
                    ["ElementData"] = Catches
                });
            }
            catch (Exception e)
            {
                TaskDialog t = new("error")
                {
                    MainContent = e.Message + "\n" + e.StackTrace
                };
                return [];
            }
        } 
    }

}
