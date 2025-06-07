using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Direwolf.Extensions;
using Nice3point.Revit.Toolkit.External;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace Direwolf.Revit.Commands;

/// <summary>
///     Exports the Direwolf Wolfden to JSON.
/// </summary>
[UsedImplicitly]
[Transaction
    (TransactionMode.Manual)]
public class ExportCacheToJson : ExternalCommand
{
    public override void Execute()
    {
        using var saveDialog = new SaveFileDialog();
        saveDialog.Filter = "JSON (*.json)|*.json";
        saveDialog.Title = "Save Result.";
        saveDialog.DefaultExt = "json";
        saveDialog.AddExtension = true;

        if (saveDialog.ShowDialog() == DialogResult.OK)
        {
            var filePath = saveDialog.FileName;
            WriteFile
                (filePath);
        }
        else
        {
            var t = new TaskDialog
                ("Exporting Cache to JSON") { MainContent = "File not saved" };
            t.Show();
            t.Dispose();
        }
    }

    private void WriteFile(string fileName)
    {
        TaskDialog t = new
            ("Exporting Results to JSON");
        Stopwatch sw = new();
        sw.Start();
        File.WriteAllText
        (fileName,
            JsonSerializer.Serialize
                (Document.GetCacheByCategory()));
        sw.Stop();
        t.MainContent = $"File saved at {fileName}Time taken: {sw.Elapsed.TotalSeconds}";
        t.Show();
    }
}