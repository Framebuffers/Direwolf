using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Direwolf.Definitions;
using Direwolf.Definitions.Enums;
using Direwolf.Definitions.LLM;
using Direwolf.Definitions.Revit;
using Nice3point.Revit.Toolkit.External;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace Direwolf.Revit.Commands.Testing;

[Transaction(TransactionMode.ReadOnly)]
public class GetElementInfo : ExternalCommand
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
            var elements = new List<RevitElement?>();
            
            var selection = UiDocument.Selection; 
            var selectedElements = selection.GetElementIds().ToElements(Document);
            foreach (var element in selectedElements)
            {
                elements.Add(RevitElement.Create(Document, element.UniqueId));
            }
            
            var wp = _message with
            {
                Name = "selected_elements",
                Description = "Get information from selected elements.",
                Result = elements
            };
            
            var filePath = saveDialog.FileName;
            WriteFile
                (filePath, JsonSerializer.Serialize(wp));
        }
        else
        {
            var t = new TaskDialog
                ("Writing info about selected elements") { MainContent = "File not saved" };
            t.Show();
            t.Dispose();
        }
    }

    private void WriteFile(string fileName, string data)
    {
        TaskDialog t = new
            ("Writing info about selected elements");
        Stopwatch sw = new();
        sw.Start();
        File.WriteAllText
            (fileName, data);
        sw.Stop();
        t.MainContent = $"File saved at {fileName}Time taken: {sw.Elapsed.TotalSeconds}";
        t.Show();
    }
}