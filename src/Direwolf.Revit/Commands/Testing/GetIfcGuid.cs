using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace Direwolf.Revit.Commands.Testing;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class GetIfcGuid : ExternalCommand
{
    public override void Execute()
    {
        using var saveDialog = new SaveFileDialog();
        saveDialog.Filter = "JsonSchemas (*.json)|*.json";
        saveDialog.Title = "Save ResultType";
        saveDialog.DefaultExt = "json";
        saveDialog.AddExtension = true;
        if (saveDialog.ShowDialog() == DialogResult.OK)
        {
            var f = new FilteredElementCollector(Document).WhereElementIsNotElementType().ToElements();
            var dict = new Dictionary<string, List<string>>();
            foreach (var element in f)
            {
                var x = element.get_Parameter(BuiltInParameter.IFC_GUID).AsString();
                var cat = element.get_Parameter(BuiltInParameter.IFC_TYPE_GUID).AsString();
                if (!dict.TryGetValue(cat, out var e))
                {
                    e = [];
                    dict[cat] = e;
                }
                dict[cat].Add(x);
            }

            var filePath = saveDialog.FileName;
            var str = JsonSerializer.Serialize(dict).ToString();
            WriteFile(filePath, str);
        }
    }

    private void WriteFile(string fileName, string data)
    {
        TaskDialog t = new("Exporting Results to JsonSchemas");
        File.WriteAllText(fileName, data);
        t.MainContent = $"File saved at {fileName}";
        t.Show();
    }
}