using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Direwolf.Definitions;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.ModelHealth;
using Direwolf.Definitions.RevitApi;
using Direwolf.Extensions;
using Microsoft.WindowsAPICodePack.Dialogs;
using Nice3point.Revit.Toolkit.External;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;
using TaskDialogResult = Autodesk.Revit.UI.TaskDialogResult;

// ReSharper disable HeapView.ObjectAllocation

namespace Direwolf.Revit.Commands.Testing;

/// <summary>
///     Test suite for Direwolf.
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.ReadOnly)]
public class TestCommands : ExternalCommand
{
    private static readonly StringWriter? StringWriter = new();
    private static string _path = string.Empty;

    /*
     * 1. Populate Database.
     *      On start, check if the DB is not null.
     *      Expected: Any value >0.
     *      Benchmark: Boolean, true if conditions are met, false otherwise.
     */
    private bool Check_PopulateDB()
    {
        WriteToConsole("Populating Database");
        return Direwolf.GetDatabase(Document)
                   .GetDatabaseCount() !=
               0;
    }

    /*
     * 2. Test Adding and Reading.
     *      Given a set of ElementId obtained from a FilteredElementCollector,
     *      add them to the DB and then read their value.
     *      Expected:  Either an add or update to the cache, and values to be
     *                 identical to those introduced.
     *      Benchmark: Boolean, true if conditions are met, false otherwise.
     *      Note:      Some values *might* change during the test.
     */
    private bool Check_ReadWriteDB()
    {
        var db = Direwolf.GetDatabase(Document);
        var doors = Document.GetElements()
            .WhereElementIsNotElementType()
            .WhereElementIsViewIndependent()
            .OfCategory(BuiltInCategory.OST_Doors)
            .ToElementIds()
            .Where(x => !x.Equals(ElementId.InvalidElementId))
            .ToList();
        WriteToConsole($"Looping through all Door instances: {doors.Count}");
        var addedUniqueItems = new List<string>();
        foreach (var door in doors)
        {
            var uuid = Document.GetElement(door).UniqueId;
            db.Add(uuid,
                Document);
            addedUniqueItems.Add(uuid);
            Debug.Print($"Added: {door.Value}");
        }

        if (addedUniqueItems.Count != doors.Count)
        {
            WriteToConsole(
                $"Mismatch between Transactions and Elements in list: Added = {addedUniqueItems.Count}. In Collector = {doors.Count}");
            return false;
        }

        WriteToConsole("Checking DB:");
        WriteToConsole("\tCount between Cached and Collected Elements");
        var readInfo = db.Read(addedUniqueItems.ToArray(), Document);
        var counterDict = new Dictionary<string, int>();
        foreach (var record in readInfo)
        {
            if (record is null) continue;
            var id = record.Value.Id.CounterSubstring + record.Value.Id.FingerprintSubstring;
            if (!counterDict.TryGetValue(record.Value.ElementUniqueId,
                    out var counter))
            {
                counter = 0;
                counterDict.Add(record.Value.ElementUniqueId,
                    counter);
            }

            counter++;
            WriteToConsole(
                $"{record.Value.ElementUniqueId}::StoredID:{id}::DocumentID:{Document.GetDocumentUuidHash()}::Counter:{counter}");
        }

        Debug.Print(JsonSerializer.Serialize(counterDict));
        WriteToConsole("Checking ElementUniqueId");
        foreach (var door in doors)
        {
            var unique = Document.GetElement(door)
                .UniqueId;
            if (!addedUniqueItems.Contains(unique))
            {
                WriteToConsole($"UniqueID not found: {unique}");
                return false;
            }

            WriteToConsole($"Found: {door.Value}");
        }

        WriteFile("test_db_read_write.json",
            readInfo.ToString(),
            out var t);
        WriteToConsole($"Time taken: {t}");
        return true;
    }

    /*
     * 3. Exporting to JSON from cache.
     *      This test serializes *the whole cache* of RevitElements to a
     *      JSON file in a directory chosen by the user.
     *      Expected:  A (big) JSON with every single Element's Parameters
     *                 using the RevitElement and RevitParameter format.
     *      Benchmark: Time taken.
     */
    private bool Check_JsonFromCache()
    {
        try
        {
            WriteFile("jsonFromCache.json",
                JsonSerializer.Serialize(Document.GetCacheByCategory()),
                out var t);
            WriteToConsole("Wrote JSON from cache.");
            WriteToConsole($"Time taken to write from Cache: {t}");
            return true;
        }
        catch (Exception e)
        {
            WriteToConsole(e.Message);
            return false;
        }
    }

    /*
     * 4. Exporting to JSON from disk.
     *      This test serializes the whole Document from scratch, creating a new
     *      RevitElement for it and sending it to a JsonSerializer, to a JSON
     *      file in a directory of the user's choice.
     *      Expected:  A (big) JSON with every single Element's Parameters
     *                 using the RevitElement and RevitParameter format.
     *      Benchmark: Time taken.
     */
    private bool Check_JsonFromDisk()
    {
        try
        {
            var db = Document.GetRevitDbByCategory();
            WriteFile("jsonFromDisk.json",
                JsonSerializer.Serialize(db),
                out var t);
            WriteToConsole("Wrote JSON from Disk");
            WriteToConsole($"Time taken to write from Disk: {t}");
            return true;
        }
        catch (Exception e)
        {
            WriteToConsole(e.Message);
            return false;
        }
    }

    /*
     * 5. Querying
     *     A specific query, similar to a SQL sentence, will be performed against
     *     the database. Results will be processed in the Revit frontend.
     *     Expected:  A JSON file containing the resulting data using the
     *                RevitElement and RevitParameter format.
     *     Benchmark: If a file is produced, and the content is correct.
     *                Time taken to perform the query.
     */
    private bool Check_DatabaseQuery()
    {
        var chosen = Document.GetElements()
            .OfCategory(BuiltInCategory.OST_Doors)
            .ToElements()
            .Select(x => RevitElement.Create(Document,
                x.UniqueId))
            .ToList();
        if (chosen.Count == 0)
            return false;
        foreach (var rvtElement in chosen)
            WriteToConsole($"Check DB Query::Found: {rvtElement?.Id}");
        using StringWriter sw = new();
        var w = DirewolfExport.Create(Document,
            "DoorIfcGUID",
            Realm.Category,
            chosen,
            new Dictionary<string, object>
            {
                ["IfcGUID"] = chosen
                    .Where(x => x is not null)
                    .Select(x => RevitElement.CreateWithSpecificParameters(Document, x!.Value.ElementUniqueId,
                        [BuiltInParameter.IFC_GUID]))
                    .ToList() 
                
                    // .Where(x => x is not null)
                    // .SelectMany(x => x!.Value.Parameters
                    //     .Where(y => y.HasValue)
                    //     .Where(y => y!.Value.Key.Equals("IfcGUID"))
                    //     .Select(y => y!.Value)
                    //     .Where(y => y.Key.Equals("IfcGUID"))
                    //     .SelectMany(y => y.Value)
                    //     .ToList()),
            });
        if (w.ToString()
            .IsNullOrEmpty())
            return false;
        StringWriter?.Write($"{w}");
        WriteFile("DTO_test.json",
            JsonSerializer.Serialize(w),
            out var time);
        WriteToConsole($"Time taken: {time}");
        return true;
    }

    private bool Check_ModelHealthIndicators()
    {
        var validEid = Document
            .GetElements()
            .WhereElementIsNotElementType()
            .WhereElementIsViewIndependent()
            .ToElements();
        
        var mh = ModelHealthIndicators.Create(Document, validEid);
        StringWriter?.Write(mh.ToString()); 
        WriteFile("model_health.json", JsonSerializer.Serialize(mh), out var time);
         WriteToConsole($"Time taken: {time}");
                return true;
    }

    public override void Execute()
    {
        using Autodesk.Revit.DB.Transaction trans = new(Document,
            "Running tests");
        var t2 = TaskDialog.Show("Executing Direwolf Tests",
            "This Command will run through five tests to determine if Direwolf is running correctly." +
            "Would you like to continue?",
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
            WriteToConsole(Check_PopulateDB()
                ? "Test passed."
                : "Test failed.");
            WriteToConsole("Running Test 2: Read and Write to DB");
            WriteToConsole(Check_ReadWriteDB()
                ? "Test passed."
                : "Test failed.");
            WriteToConsole("Running Test 3: JSON to Cache");
            WriteToConsole(Check_JsonFromCache()
                ? "Test passed."
                : "Test failed.");
            WriteToConsole("Running Test 4: JSON to Disk");
            WriteToConsole(Check_JsonFromDisk()
                ? "Test passed."
                : "Test failed.");
            WriteToConsole("Running Test 5: Query (IFC GUID to JSON)");
            WriteToConsole(Check_DatabaseQuery()
                ? "Test passed."
                : "Test failed.");
            WriteToConsole("Running test 6: Model Health Check");
            WriteToConsole(Check_ModelHealthIndicators()
                ? "Test passed"
                : "Test failed.");
            WriteToConsole($"Tests finished at {DateTime.UtcNow}");
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