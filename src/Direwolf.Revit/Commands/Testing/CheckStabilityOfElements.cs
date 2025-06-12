using System.Diagnostics;
using System.IO;
using System.Runtime.Caching;
using System.Text.Json;
using Autodesk.Revit.Attributes;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.RevitApi;
using Nice3point.Revit.Toolkit.External;

namespace Direwolf.Revit.Commands.Testing;

[Transaction(TransactionMode.ReadOnly)]
public class CheckStabilityOfElements : ExternalCommand
{
    public override void Execute()
    {
        var selection = this.UiDocument.Selection;
        using var sw = new StringWriter();
        var selectedElements = selection.GetElementIds().ToElements(Document);
        foreach (var element in selectedElements)
        {
            var e= RevitElement.Create(Document, element.UniqueId);
            sw.Write(JsonSerializer.Serialize(e));
        }
        Debug.Print(sw.ToString());
    }
}