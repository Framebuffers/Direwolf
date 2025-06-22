using System.Diagnostics;
using System.IO;
using System.Windows.Forms.VisualStyles;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Events;
using Direwolf.Definitions.LLM;
using Direwolf.Driver.MCP;
using Microsoft.WindowsAPICodePack.Dialogs;
using Nice3point.Revit.Toolkit.External;
using Revit.Async;
using Wpf.Ui.Controls;
using static Autodesk.Revit.UI.TaskDialogCommonButtons;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;
using TaskDialogResult = Autodesk.Revit.UI.TaskDialogResult;
using TextBox = System.Windows.Controls.TextBox;

namespace Direwolf.Revit.Commands.Testing;

[Transaction(TransactionMode.Manual)]
public class AnthopicClient : ExternalCommand
{
    private McpDriver? _driver;
    private readonly Hunter? _hunter = Hunter.GetInstance(Direwolf.GetInstance());
    private string? _anthropicApiKey;
    private static string? _path;
    private readonly TextReader? _reader = null;
    private readonly TextWriter? _writer = null;
    
    public override void Execute()
    {
        var t2 = TaskDialog.Show("Running MCP Test",
            "This Command will ask for an Anthropic API Key to continue. Refer to documentation for more info." +
            " Would you like to continue?",
            Yes | No);
        
        if (TaskDialogResult.Yes == t2) 
            GetKeyPath();
        else if (TaskDialogResult.No == t2)
            TaskDialog.Show("Running MCP Test",
                "Test cancelled.");
        
        _anthropicApiKey = File.ReadAllText(_path!);
        
        
        var panel = UiApplication.GetRibbonPanels("Direwolf").FirstOrDefault(x => x.Name.Equals("MCP"));
        var promptInput = panel!.AddTextBox("promptIn");
        promptInput.PromptText = "Enter your prompt here.";
         promptInput.EnterPressed += (_, __) =>
         {
             if (_hunter is null || string.IsNullOrEmpty(_anthropicApiKey))
                 throw new NullReferenceException("AnthropicApiKey is null or empty.");
             _driver = McpDriver.GetInstance(_hunter, _anthropicApiKey);

             var wp = WolfpackMessage.Create("wolfpack://direwolf/hunter/tools/llm/analyze", null) with
             {
                 Parameters = new Dictionary<string, object>
                 {
                     ["method"] = "initialize"
                 }
             };

             Debug.WriteLine("MCP Host listening.");

             McpDriver.GetInstance(_hunter, _anthropicApiKey).HandleRequest(wp);
         };
    }

    private void OnPromptInputOnEnterPressed(object? s, TextBoxEnterPressedEventArgs e)
    {
        var textbox = s as TextBox;
        if (textbox is not null) textbox.Text = string.Empty;
        var enteredText = textbox?.Text;

        if (!string.IsNullOrWhiteSpace(enteredText))
        {
            Debug.WriteLine(enteredText);
            McpDriver.ToConsole(enteredText);
        }

        textbox!.Text = string.Empty;
    }

    private static void GetKeyPath()
    {
        CommonOpenFileDialog d = new();
        d.InitialDirectory = "C:\\Users";
        d.IsFolderPicker = false;
        if (d.ShowDialog() == CommonFileDialogResult.Ok)
            _path = Path.GetFullPath(d.FileName);
    }


}
