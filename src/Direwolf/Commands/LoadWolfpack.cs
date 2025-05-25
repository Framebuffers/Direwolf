using System.Windows;

using Autodesk.Revit.Attributes;

using Direwolf.Definitions.Revit;

using Nice3point.Revit.Toolkit.External;

namespace Direwolf.Commands;
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class LoadWolfpack : ExternalCommand
{
    
    
    public override void Execute()
    {
        using var openFileDialog = new OpenFileDialog();
        openFileDialog.Title = "Load Wolfpack file";
        openFileDialog.Filter = "Wolfpack (*.yml)|*.yml";
        this.
        
        openFileDialog.ShowDialog();
        
        // ReSharper disable once InvertIf
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            string filePath = openFileDialog.FileName;
            Autodesk.Revit.UI.TaskDialog t = new("Loaded file")
            {
                MainContent = filePath
            };
            t.Show();
        }
    }
}