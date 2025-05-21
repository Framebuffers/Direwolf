using System.Diagnostics;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Windows;

using Direwolf.Dto;
using Direwolf.Dto.InternalDb.Enums;
using Direwolf.Dto.Parser;
using Direwolf.Dto.RevitApi;

using Microsoft.Extensions.Caching.Memory;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

using Form = System.Windows.Forms.Form;
using TextBox = System.Windows.Forms.TextBox;
using Transaction = Direwolf.Dto.InternalDb.Transaction;

#pragma warning disable VISLIB0001
namespace Direwolf.RevitUI.Commands;

[Transaction(TransactionMode.Manual)]
public class ReadWolfpack : IExternalCommand
{
    
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var file = GetFileDialog();
        var path = File.Open(file,
                             FileMode.Open,
                             FileAccess.Read,
                             FileShare.Read);

        using var reader = new StreamReader(path);
        var builder = new DeserializerBuilder();
        builder.WithNamingConvention(UnderscoredNamingConvention.Instance);
        var result = builder.Build().Deserialize<object?>(reader);
        
        Debug.Print("Reading Wolfpack");
        Debug.Print(result?.ToString());
            
        return Result.Succeeded;
        
    }

    private static WolfDto CreateDto(Transaction t, string name, Realm realm, BuiltInCategory category, out Cuid id)
    {
        /*
         * CUID structure:
         * `clbvi4441000007ld63liebkf`
         *
         *  c           CUID v1 identifier.
         *  lbvi4441    UNIX Timestamp in milliseconds
         *  0000        Session counter
         *  07ld        Client fingerprint  (host process identifier + system hostname)
         *  63liebkf    Random data
         */

        var transactionId = Cuid.Create();
        id = transactionId;

        return new WolfDto(transactionId,
                           name,
                           realm,
                           category) { Data = t };
    }
  
    private string GetFileDialog()
    {
        var openFileDialog = new OpenFileDialog();
        try {
            openFileDialog.Title = "Select Wolfpack";
            openFileDialog.Filter = "Wolfpack|*.yml";
            openFileDialog.InitialDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            return openFileDialog.ShowDialog() == DialogResult.OK ? openFileDialog.FileName : string.Empty;
        }
        finally { openFileDialog.Dispose(); }
    }


 
    
}