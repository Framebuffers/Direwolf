using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Direwolf.Definitions;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.LLM;
using Direwolf.Extensions;
using Nice3point.Revit.Toolkit.External;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace Direwolf.Revit.Commands;

/// <summary>
///     Exports the Revit <see cref="Document" /> to JsonSchemas.
/// </summary>
[UsedImplicitly]
[Transaction
    (TransactionMode.Manual)]
public class ExportToJsonFromScratch : ExternalCommand
{
    private readonly WolfpackMessage _message = new(
        Cuid.Create().Value!,
        "Direwolf Self Test",
        "object",
        "object",
        1,
        MessageResponse.Result.ToString(),
        null,
        $"{GlobalDictionary.DirewolfSelfTest}/"); 
    public override void Execute()
    {
        using var saveDialog = new SaveFileDialog();
        saveDialog.Filter = "JsonSchemas (*.json)|*.json";
        saveDialog.Title = "Save ResultType";
        saveDialog.DefaultExt = "json";
        saveDialog.AddExtension = true;

        if (saveDialog.ShowDialog() == DialogResult.OK)
        {
            var database = Document.GetRevitDbByCategory();

            Dictionary<string, object> dictionary = database.ToDictionary(pair => pair.Key.ToString(), object (pair) => pair.Value);
            var wp = _message with
            {
                Name = "json_from_disk",
                Description = "Get the whole Revit Document to a JSON file.",
                Result = dictionary
            };
            
            
            var filePath = saveDialog.FileName;
            WriteFile
                (filePath, JsonSerializer.Serialize(wp));
        }
        else
        {
            var t = new TaskDialog
                ("Exporting Cache to JsonSchemas") { MainContent = "File not saved" };
            t.Show();
            t.Dispose();
        }
    }

    private void WriteFile(string fileName, string data)
    {
        TaskDialog t = new
            ("Exporting Results to JsonSchemas");
        Stopwatch sw = new();
        sw.Start();
        File.WriteAllText
        (fileName, data);
        sw.Stop();
        t.MainContent = $"File saved at {fileName}Time taken: {sw.Elapsed.TotalSeconds}";
        t.Show();
    }
}