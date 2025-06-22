using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Direwolf.Definitions;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.LLM;
using Direwolf.Definitions.PlatformSpecific;
using Direwolf.Definitions.PlatformSpecific.Extensions;
using Direwolf.Extensions;
using Microsoft.WindowsAPICodePack.Dialogs;
using Nice3point.Revit.Toolkit.External;
using Exception = System.Exception;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;
using TaskDialogResult = Autodesk.Revit.UI.TaskDialogResult;

// ReSharper disable HeapView.ObjectAllocation

namespace Direwolf.Revit.Commands.Testing;

//TODO: adapt to new API
/*
 * the new test suite should test:
 * - populating Wolfden and test if the objects match the Revit schema
 * - check if the elements on screen can be serialized to a *single* howl.
 */
/// <summary>
///     Test suite for Direwolf.
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.ReadOnly)]
public class TestCommands : ExternalCommand
{
    private static readonly StringWriter? StringWriter = new();
    private static string _path = string.Empty;
    private static List<Wolfpack> _results = [];
    private const string Uri = "wolfpack://com.revit.autodesk-2025/direwolf/custom?t=";

    private readonly WolfpackMessage _message = new(
        "",
        "",
        "object",
        1,
        "",
        ResultType.Rejected.ToString(),
        $"{Uri}/",
        null);
    
    /*
     * 1. Populate Database.
     *      On start, check if the DB is not null.
     *      Expected: Any value >0.
     *      Benchmark: MessageResponse.ResultType if conditions are met, MessageResponse.Error otherwise.
     */
    private MessageResponse Check_PopulateDB()
    {
        WriteToConsole("Populating Database");
        Direwolf.GetAllElements(Document, out var db);
        WriteToConsole($"Found {db!.Count} elements");
        return db.Count != 0 ? MessageResponse.Result : MessageResponse.Error;
    }

    /*
     * 2. Test Adding and Reading.
     *      Given a set of ElementId obtained from a FilteredElementCollector,
     *      add them to the DB and then read their value.
     *      Expected:  Either an add or update to the cache, and values to be
     *                 identical to those introduced.
     *      Benchmark: MessageResponse.ResultType if conditions are met, MessageResponse.Error otherwise.
     *      Note:      Some values *might* change during the test.
     */
    private MessageResponse Check_ElementCache()
    {
        var doors = Document.GetElements()
            .WhereElementIsNotElementType()
            .WhereElementIsViewIndependent()
            .OfCategory(BuiltInCategory.OST_Doors)
            .ToElementIds()
            .Where(x => !x.Equals(ElementId.InvalidElementId))
            .ToArray();
        
        WriteToConsole($"Looping through all Door instances: {doors.Length}");
        
        List<string> uuids = [];
        foreach (var door in doors)
        {
            var uuid = Document.GetElement(door).UniqueId;
            uuids.Add(uuid);
            Debug.Print($"Added: {door.Value}");
        }
        
        WriteToConsole("Checking DB:");
         var wp = _message with
                    {
                        Name = "json_from_wolfden",
                        Description = "Get the whole Revit Document from the local cache.",
                        Result = ResultType.Accepted.ToString(),
                        Parameters = new Dictionary<string, object>
                        {
                            ["key"] = uuids
                        }
                    };
         
        var read = Direwolf.GetInstance().ReadRevitElements(doors, Document, out _);
        if (read is MessageResponse.Error) return MessageResponse.Error;
        var wolfpack = Wolfpack.Create("dwolf_test", MessageResponse.Result, RequestType.Get, WolfpackMessage.ToDictionary(wp));

        WriteFile("Test02_CacheQuery.json",
            JsonSerializer.Serialize(wolfpack),
            out var t);
        WriteToConsole($"Time taken: {t}");
        
        _results.Add(wolfpack);
        return MessageResponse.Result;
    }

    /*
     * 3. Exporting to JsonSchemas from cache.
     *      This test serializes *the whole cache* of RevitElements to a
     *      JsonSchemas file in a directory chosen by the user.
     *      Expected:  A (big) JsonSchemas with every single Element's Params
     *                 using the RevitElement and RevitParameter format.
     *      Benchmark: Time taken.
     */
    private MessageResponse Check_JsonFromWolfden()
    {
        try
        {
            Direwolf.GetAllElements(Document, out var dictionary);
            var wp = _message with
            {
                Name = "json_from_wolfden",
                Description = "Get the whole Revit Document from the local cache.",
                Result = ResultType.Accepted.ToString(),
                Parameters = dictionary
            };
            
            var wolfpack = Wolfpack.Create("dwolf_test", MessageResponse.Result, RequestType.Get, WolfpackMessage.ToDictionary(wp));
            
            WriteFile("Test03_CheckCachedElements.json",
                JsonSerializer.Serialize(wolfpack),
                out var t);
            WriteToConsole("Wrote JsonSchemas from Disk");
            WriteToConsole($"Time taken to write from Disk: {t}");
            _results.Add(wolfpack with
            {
                Parameters = new Dictionary<string, object>
                {
                    ["properties"] = wolfpack.Parameters!.Count
                }
            });
            return MessageResponse.Notification;
        }
        catch (Exception e)
        {
            WriteToConsole(e.Message);
            return MessageResponse.Error;
        }
    }

    /*
     * 4. Exporting to JsonSchemas from disk.
     *      This test serializes the whole Document from scratch, creating a new
     *      RevitElement for it and sending it to a JsonSerializer, to a JsonSchemas
     *      file in a directory of the user's choice.
     *      Expected:  A (big) JsonSchemas with every single Element's Params
     *                 using the RevitElement and RevitParameter format.
     *      Benchmark: Time taken.
     */
    //TODO: benchmark getting data from hunter, it doesn't really matter if I serialize from Wolfden-- it's internal.
    private MessageResponse Check_JsonFromDisk()
    {
        try
        {
            var database = Document.GetRevitDbByCategory();

            Dictionary<string, object> dictionary = database.ToDictionary(pair => pair.Key.ToString(), pair => (object)pair.Value);
            var wp = _message with
            {
                Name = "json_from_disk",
                Description = "Get the whole Revit Document to a JSON file.",
                Result = ResultType.Accepted.ToString(),
                Parameters = dictionary
            };
            
            var wolfpack = Wolfpack.Create("dwolf_test", MessageResponse.Result, RequestType.Get, WolfpackMessage.ToDictionary(wp));
            
            WriteFile("Test04_CheckDocumentElements.json",
                JsonSerializer.Serialize(wolfpack),
                out var t);
            WriteToConsole("Wrote JsonSchemas from Disk");
            WriteToConsole($"Time taken to write from Disk: {t}");
            _results.Add(wolfpack with
            {
                Parameters = new Dictionary<string, object>
                {
                    ["properties"] = wolfpack.Parameters!.Count
                }
            });
            return MessageResponse.Notification;
        }
        catch (Exception e)
        {
            WriteToConsole(e.Message);
            return MessageResponse.Error;
        }
    }

    /*
     * 5. Querying
     *     A specific query, similar to a SQL sentence, will be performed against
     *     the database. Results will be processed in the Revit frontend.
     *     Expected:  A JsonSchemas file containing the resulting data using the
     *                RevitElement and RevitParameter format.
     *     Benchmark: If a file is produced, and the content is correct.
     *                Time taken to perform the query.
     */
    private MessageResponse Check_DatabaseQuery()
    {
        var chosen = Document.GetElements()
            .OfCategory(BuiltInCategory.OST_Doors)
            .ToElements()
            .Select(x => RevitElement.Create(Document,
                x.UniqueId))
            .ToList();
      
        foreach (var rvtElement in chosen)
            WriteToConsole($"Check DB Query::Found: {rvtElement?.Id}");
        
        using StringWriter sw = new();
       
        var x = chosen.ToDictionary(x => x!.Value.ElementUniqueId,
            x => (object)x!.Value.Parameters.First(y => y is not null && y.Value.Key.Contains("IfcGUID"))!);
        
        var wp = _message with
        {
            Name = "get_door_ifc_guid",
            Description = "Get the IFC GUID of all the Doors inside Revit.",
            Result = ResultType.Accepted.ToString(),
            Parameters = x
        };
            
        var wolfpack = Wolfpack.Create("dwolf_test", MessageResponse.Result, RequestType.Get, WolfpackMessage.ToDictionary(wp));
    
        WriteFile("Test05_QueryDB.json",
            JsonSerializer.Serialize(wolfpack),
            out var time);
        WriteToConsole($"Time taken: {time}");
        
        _results.Add(wolfpack with
        {
            Parameters = new Dictionary<string, object>
            {
                ["properties"] = wolfpack.Parameters!.Count
            }
        });
        return MessageResponse.Result;
    }

    private MessageResponse Check_CategoryAsJsonl()
    {
        var x = ExternalCommandData.Application.ActiveUIDocument.ActiveView;
        var windows = Document.GetElements()
                    .WhereElementIsNotElementType()
                    .WhereElementIsViewIndependent()
                    .OfCategory(BuiltInCategory.OST_Windows)
                    .ToElementIds()
                    .Where(x => !x.Equals(ElementId.InvalidElementId))
                    .ToArray();
        var jsonl = x.ElementsOfCategoryInViewToJsonl(Document, Document.GetElement(windows[0]).Category);
        var wp = _message with
                {
                    Name = "category_as_jsonl",
                    Description = "Get all windows as a JSONL.",
                    Result = ResultType.Accepted.ToString(),
                    Parameters = new Dictionary<string, object>
                    {
                        ["result"] = jsonl
                    }
                };
                    
                var wolfpack = Wolfpack.Create("dwolf_test", MessageResponse.Result, RequestType.Get, WolfpackMessage.ToDictionary(wp));
                WriteFile("Test06_WindowsToJsonl.json",
                    JsonSerializer.Serialize(wolfpack),
                    out var time);
                WriteToConsole($"Time taken: {time}");
        
                _results.Add(wolfpack with
                {
                    Parameters = new Dictionary<string, object>
                    {
                        ["properties"] = wolfpack.Parameters!.Count
                    }
                });
                return MessageResponse.Result; 
    }

  

    public override void Execute()
    {
        using Transaction trans = new(Document,
            "Running tests");
        var t2 = TaskDialog.Show("Executing Direwolf Tests",
            "This Command will run through five tests to determine if Direwolf is running correctly." +
            " Would you like to continue?",
            TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No);
        if (TaskDialogResult.Yes == t2)
            RunTests();
        else if (TaskDialogResult.No == t2)
            TaskDialog.Show("Direwolf Test Suite",
                "Tests cancelled.");
    }

    private void RunTests()
    {
        try
        {
            GetSavePath();
            WriteToConsole("Direwolf Testing Suite for v0.2.1-beta 2025-06-19");
            WriteToConsole("Running Test 1: Checking Cache Population");
            WriteToConsole(Check_PopulateDB().ToString());
            WriteToConsole("Running Test 2: Check ElementCache integrity");
            WriteToConsole(Check_ElementCache().ToString());
            WriteToConsole("Running Test 3: Serialize ElementCache to JSON)");
            WriteToConsole(Check_JsonFromWolfden().ToString());
            WriteToConsole("Running Test 4: Serialize Document to JSON");
            WriteToConsole(Check_JsonFromDisk().ToString());
            WriteToConsole("Running Test 5: Get all IFC GUID's from all Doors");
            WriteToConsole(Check_DatabaseQuery().ToString());
            WriteToConsole("Running Test 6: Serialize all windows to JSONL");
            WriteToConsole(Check_CategoryAsJsonl().ToString());


            var args = _message with
            {
                Name = "dwolf_selftest",
                Description= "Runs a series of tests on Direwolf to check: \nCache is populated.\nElementCache is populated\nCan read the DB both from Cache and Document\nRun a query over the elements of its cache." +
                             "\nThis makes sure that all elements of Direwolf are working: caching, querying and serialization.",
                Result = ResultType.Accepted.ToString(),
                Parameters = _results.ToDictionary(x => x.Name, x => (object)x)
            };
            var wolfpackAll = Wolfpack.Create("results", MessageResponse.Result, RequestType.Get, WolfpackMessage.ToDictionary(args), $"Completed at {DateTime.UtcNow}");
            
            WriteFile("wolfpack.json", JsonSerializer.Serialize(wolfpackAll), out var time);
            WriteToConsole($"Tests finished at {DateTime.UtcNow}, time: {time}");
        }
        catch (Exception e)
        {
            StringWriter?.WriteLine(e.Message);
            WriteFile("logs.txt",
                StringWriter?.ToString(),
                out var t);
            Debug.Print($"Time taken: {t}");
            throw;
        }
    }

    private static void GetSavePath()
    {
        CommonOpenFileDialog d = new();
        d.InitialDirectory = "C:\\Users";
        d.IsFolderPicker = true;
        if (d.ShowDialog() == CommonFileDialogResult.Ok)
            _path = Path.GetFullPath(d.FileName);
    }

    private static void WriteFile(string fileName,
        string? content,
        out double? timeSeconds)
    {
        try
        {
            Stopwatch sw = new();
            sw.Start();
            File.WriteAllText(Path.Combine(_path,
                    fileName),
                content);
            sw.Stop();
            timeSeconds = sw.Elapsed.TotalSeconds;
        }
        catch
        {
            timeSeconds = null;
        }
    }

    private static void WriteToConsole(string? message)
    {
        Debug.Print(message);
        StringWriter?.WriteLine(message);
    }
}