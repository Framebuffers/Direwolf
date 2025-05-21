using System.Diagnostics;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Form = System.Windows.Forms.Form;
using TextBox = System.Windows.Forms.TextBox;

#pragma warning disable VISLIB0001
namespace Direwolf.RevitUI.WinForms;

[Transaction(TransactionMode.Manual)]
public class CreateTextBox : IExternalCommand
{
    
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        Debug.Print(PromptForText());
        return Result.Succeeded;
        
    }
  
    private string PromptForText()
    {
        using (Form form = new Form())
        {
            Label label = new Label() { Text = "Enter text:", Left = 10, Top = 10 };
            TextBox textBox = new TextBox() { Left = 10, Top = 30, Width = 200 };
            Button buttonOk = new Button() { Text = "OK", Left = 10, Top = 60 };

            buttonOk.Click += (sender, e) => { form.DialogResult = DialogResult.OK; form.Close(); };

            form.Controls.Add(label);
            form.Controls.Add(textBox);
            form.Controls.Add(buttonOk);
            form.StartPosition = FormStartPosition.CenterScreen;
            form.AcceptButton = buttonOk;

            return form.ShowDialog() == DialogResult.OK ? textBox.Text : string.Empty;
        }
    }
}
