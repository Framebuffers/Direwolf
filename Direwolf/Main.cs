using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Revit.Async;
using System.Text.Json;
using System.Diagnostics;
using Direwolf.Examples.Howlers;

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
            // This is a benchmark of data extraction between a native syncronous Revit Command, and a Direwolf asyncronous Command.
            // The one difference from any other Revit.Async command, is that Direwolf has a special data structure.
            // Here's a native Revit command:

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
            WriteToFile("elementsByFamily-native.json", elementsSortedByFamilyNative);

            // Add results from native Revit command.
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
            dw.ExecuteQueryAsync(new RevitDocumentDispatch(doc), "DocumentInformation");
            
            // Add results from Direwolf
            direwolf.Stop();
            Results.Add("Direwolf Revit Command", direwolf.Elapsed.TotalSeconds);
            
            // Show final results
            TaskDialog t = new("Results")
            {
                MainContent = JsonSerializer.Serialize(Results)
            };
            t.Show();
            return Result.Succeeded;
        }

        public void WriteToFile(string filename, object obj)
        {
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), filename);
            File.WriteAllText(fileName, JsonSerializer.Serialize(obj));
        }
    }
}
