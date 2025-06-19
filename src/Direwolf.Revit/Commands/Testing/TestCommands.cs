using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Direwolf.Definitions;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.LLM;
using Direwolf.Definitions.PlatformSpecific;
using Direwolf.Definitions.PlatformSpecific.Records;
using Direwolf.Definitions.Serialization;
using Direwolf.Extensions;
using Microsoft.WindowsAPICodePack.Dialogs;
using Nice3point.Revit.Toolkit.External;
using Exception = System.Exception;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;
using TaskDialogResult = Autodesk.Revit.UI.TaskDialogResult;

// ReSharper disable HeapView.ObjectAllocation

namespace Direwolf.Revit.Commands.Testing;

//TODO: adapt to new API
/// <summary>
///     Test suite for Direwolf.
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.ReadOnly)]
public class TestCommands : ExternalCommand
{
    private static readonly StringWriter? StringWriter = new();
    private static string _path = string.Empty;
    private static List<Howl> _results = [];
    private const string Uri = "wolfpack://com.revit.autodesk-2025/direwolf/custom?t=";
    /*
     * 1. Populate Database.
     *      On start, check if the DB is not null.
     *      Expected: Any value >0.
     *      Benchmark: MessageType.ResultType if conditions are met, MessageType.Error otherwise.
     */
    private MessageType Check_PopulateDB()
    {
        WriteToConsole("Populating Database");
        Direwolf.GetElementCache(Document, out var db);
        return db == null ? MessageType.Error : MessageType.Result;
    }

    /*
     * 2. Test Adding and Reading.
     *      Given a set of ElementId obtained from a FilteredElementCollector,
     *      add them to the DB and then read their value.
     *      Expected:  Either an add or update to the cache, and values to be
     *                 identical to those introduced.
     *      Benchmark: MessageType.ResultType if conditions are met, MessageType.Error otherwise.
     *      Note:      Some values *might* change during the test.
     */
    private MessageType Check_ElementCache()
    {
        var doors = Document.GetElements()
            .WhereElementIsNotElementType()
            .WhereElementIsViewIndependent()
            .OfCategory(BuiltInCategory.OST_Doors)
            .ToElementIds()
            .Where(x => !x.Equals(ElementId.InvalidElementId))
            .ToList();
        WriteToConsole($"Looping through all Door instances: {doors.Count}");
        
        List<string> uuids = [];
        foreach (var door in doors)
        {
            var uuid = Document.GetElement(door).UniqueId;
            uuids.Add(uuid);
            Debug.Print($"Added: {door.Value}");
        }
        
        WriteToConsole("Checking DB:");
        Direwolf.GetInstance().ReadRevitElements(uuids.ToArray(), Document, out var rvtElements, out var h);
        
        var wolfpack = Wolfpack.Create(
            Cuid.CreateRevitId(Document, out var _), 
            RequestType.Get,
            "Doors",
            WolfpackParams.Create(h!.Value, $"{Uri}getdoors"),
            [new McpResourceContainer("array", rvtElements.ToArray())],
            "Gets the complete Revit database as RevitElement, and serializes each element to JsonSchemas."); 

        WriteFile("Test02_CheckElementCache.json",
            JsonSerializer.Serialize(wolfpack),
            out var t);
        WriteToConsole($"Time taken: {t}");
        
        _results.Add(h.Value);
        return MessageType.Result;
    }

    /*
     * 3. Exporting to JsonSchemas from cache.
     *      This test serializes *the whole cache* of RevitElements to a
     *      JsonSchemas file in a directory chosen by the user.
     *      Expected:  A (big) JsonSchemas with every single Element's Params
     *                 using the RevitElement and RevitParameter format.
     *      Benchmark: Time taken.
     */
    private MessageType Check_JsonFromWolfden()
    {
        try
        {
            Direwolf.GetElementCache(Document, out var elements);
            var howl = Howl.Create(DataType.Array, RequestType.Get, elements!.ToDictionary()) with
            {
                Result = ResultType.Accepted
            };
            var wolfpack = Wolfpack.Create(RequestType.Get, "database",
                WolfpackParams.Create(howl, $"{Uri}databaseFromCache"), [McpResourceContainer.Create(howl)], null);
            
            WriteFile("Test03_JsonFromWolfden.json",
                JsonSerializer.Serialize(wolfpack),
                out var t);
            WriteToConsole("Wrote JsonSchemas from Wolfden.");
            WriteToConsole($"Time taken to write from Wolfden: {t}");
            _results.Add(howl with
            {
                Properties = new()
                {
                    ["result"] = howl.Properties!.Count
                }
            });
            return MessageType.Notification;
        }
        catch (Exception e)
        {
            WriteToConsole(e.Message);
            return MessageType.Error;
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
    private MessageType Check_JsonFromDisk()
    {
        try
        {
            var database = Document.GetRevitDbByCategory();
            var howl = Howl.Create(DataType.Array, RequestType.Get, database.ToDictionary(x => x.Key.ToString(), x => (object)x.Value!), Document.Title) with
            {
                Result = ResultType.Accepted
            };
            var wolfpack = Wolfpack.Create(Cuid.CreateRevitId(Document, out var _), RequestType.Get, "GetElements",
                WolfpackParams.Create(howl, $"{Uri}jsonFromDisk"),
                [McpResourceContainer.Create(howl)], "Result from query");  
            
            WriteFile("jsonFromDisk.json",
                JsonSerializer.Serialize(wolfpack),
                out var t);
            WriteToConsole("Wrote JsonSchemas from Disk");
            WriteToConsole($"Time taken to write from Disk: {t}");
            _results.Add(howl with
            {
                Properties = new()
                {
                    ["result"] = howl.Properties!.Count
                }
            });
            return MessageType.Notification;
        }
        catch (Exception e)
        {
            WriteToConsole(e.Message);
            return MessageType.Error;
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
    private MessageType Check_DatabaseQuery()
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

        var howl = Howl.Create(DataType.Array, RequestType.Get, x) with
        {
            Result   = ResultType.Accepted
        };

        var wolfpack = Wolfpack.Create(Cuid.CreateRevitId(Document, out _), RequestType.Get, "GetIfcGuid",
            WolfpackParams.Create(howl, $"{Uri}getIfcGuid"),
            [Howl.AsPayload(howl)], "This prompt gets all the IfcGUID from each door inside the model.");

        if (wolfpack.Data is null) return MessageType.Error;
        
        WriteFile("DTO_test.json",
            JsonSerializer.Serialize(wolfpack),
            out var time);
        WriteToConsole($"Time taken: {time}");
        
        _results.Add(howl with
        {
            Properties = new()
            {
                ["result"] = howl.Properties!.Count
            }
        });
        return MessageType.Result;
    }

    private MessageType Check_ModelHealthIndicators()
    {
        var validEid = Document
            .GetElements()
            .WhereElementIsNotElementType()
            .WhereElementIsViewIndependent()
            .ToElements();

        var wolfpack = ModelHealthIndicators.Create(Document, validEid);
        WriteFile("model_health.json", JsonSerializer.Serialize(wolfpack), out var time);
        WriteToConsole($"Time taken: {time}");
        
        return MessageType.Result;
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
            WriteToConsole("Direwolf Testing Suite for v0.4-alpha 2025-05-27");
            WriteToConsole("Running Test 1: Populating DB");
            WriteToConsole(Check_PopulateDB().ToString());
            WriteToConsole("Running Test 2: ReadRevitElements and Write to DB");
            WriteToConsole(Check_ElementCache().ToString());
            WriteToConsole("Running Test 3: JsonSchemas to Cache)");
            WriteToConsole(Check_JsonFromWolfden().ToString());
            WriteToConsole("Running Test 4: JsonSchemas to Disk");
            WriteToConsole(Check_JsonFromDisk().ToString());
            WriteToConsole("Running Test 5: Query (IFC GUID to JsonSchemas)");
            WriteToConsole(Check_DatabaseQuery().ToString());
            WriteToConsole("Running test 6: Model Health Check");
            WriteToConsole(Check_ModelHealthIndicators().ToString());

            var args = new WolfpackParams(
                "direwolfSelfCheck",
                "Tests several use cases for Direwolf: populating the DB, reading and writing, getting a JsonSchemas of all Elements from Cache and from disk, and performing a Model Health Check.",
                false,
                "query",
                1,
                MessageType.Result.ToString(),
                Result.Succeeded.ToString(),
                $"{Uri}direwolfSelfCheck");

            var wolfpackAll = Wolfpack.Create(RequestType.Get, "direwolfTestSuite", args,
                McpResourceContainer.Create(_results.ToArray()), "Results from all tests");
            
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