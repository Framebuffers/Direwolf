using System.Diagnostics;
using Autodesk.Revit.DB;
using Direwolf.EventHandlers;
using Direwolf.Revit.Definitions.Primitives;
using IConnector = Direwolf.Contracts.IConnector;

namespace Direwolf.Revit;

public class RevitDirewolf : Direwolf
{
    public Document _doc;

    protected RevitDirewolf(Document doc)
    {
        HuntCompleted += OnHuntCompleted;
        _doc = doc;
    }

    public static RevitDirewolf CreateInstance(Document doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var instance = new RevitDirewolf(doc);
        return instance;
    }

    private void OnHuntCompleted(object? sender, HuntCompletedEventArgs e)
    {
        foreach (var connector in ProcessedResults)
        {
            Debug.Print("Loading data");
            connector.Key.Create(connector.Value);
        }
    }

    public void CreateWolf(RevitHowl instruction, IConnector destination)
    {
        RevitWolf w = new(this, instruction, destination, _doc);
        w.Instruction.Wolf = w;
        WolfQueue.Add(w);
    }

    public void CreateWolf(RevitHowl instruction, List<IConnector> destinations)
    {
        foreach (var destination in destinations)
        {
            Debug.Print("Creating Wolf");
            RevitWolf w = new(this, instruction, destination, _doc);
            w.Instruction.Wolf = w; // Join instruction with runner.
            WolfQueue.Add(w);
        }
    }

    public override Task Howl()
    {
        Debug.Print("Init Howl");
        foreach (var wolf in WolfQueue) wolf.Hunt();
        HuntCompleted?.Invoke(this, null);
        return Task.CompletedTask;
    }

    public event EventHandler<HuntCompletedEventArgs>? HuntCompleted;
}