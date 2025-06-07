using System.Diagnostics;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Direwolf.Revit.Commands.Testing;

public static class TestingExtensions
{
    public static void WriteToDisk(this string? content, string path, string filename, out double? timeTaken)
    {
        try
        {
            Stopwatch sw = new();
            sw.Start();
            File.WriteAllText(Path.Combine(path, filename), content);
            sw.Stop();
            timeTaken = sw.Elapsed.TotalSeconds;
        }
        catch
        {
            timeTaken = null;
        }
    }

    public static string? GetPathStringFromDialogBox(this string initialDirectory, bool isFolderPicker = false)
    {
        CommonOpenFileDialog d = new();
        d.InitialDirectory = initialDirectory;
        d.IsFolderPicker = isFolderPicker;
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (d.ShowDialog() == CommonFileDialogResult.Ok)
            return d.FileName;
        return null;
    }
    
    
}