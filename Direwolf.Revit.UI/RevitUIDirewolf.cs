using System.Diagnostics;
using Autodesk.Revit.UI;
using Direwolf.EventHandlers;
using Direwolf.Revit.Contracts;
using Direwolf.Revit.Definitions.Primitives;
using Direwolf.Revit.UI.EventHandlers;
using Revit.Async;
using IConnector = Direwolf.Contracts.IConnector;

namespace Direwolf.Revit.UI;

public class RevitUIDirewolf : Direwolf
{
    private UIApplication _app;

    protected RevitUIDirewolf()
    {
        HuntCompleted += OnHuntCompleted;
    }

    public static RevitUIDirewolf CreateInstance(UIApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);
        var instance = new RevitUIDirewolf { _app = app };
        return instance;
    }

    private void OnHuntCompleted(object? sender, HuntCompletedEventArgs e)
    {
        foreach (var connector in ProcessedResults)
        {
            Debug.Print("Loading data");
            connector.Key.Write(connector.Value);
        }
    }

    public void CreateWolf(RevitHowl instruction, IConnector destination)
    {
        RevitWolf w = new(this, instruction, destination, _app.ActiveUIDocument.Document);
        w.Instruction.Wolf = w;
        WolfQueue.Add(w);
    }

    public void CreateWolf(RevitHowl instruction, List<IConnector> destinations)
    {
        foreach (var w in destinations.Select(destination => new RevitWolf(this,
                     instruction,
                     destination,
                     _app.ActiveUIDocument.Document)))
        {
            w.Instruction.Wolf = w; // Join instruction with runner.
            WolfQueue.Add(w);
        }
    }

    public override async Task Howl()
    {
        Debug.Print("Init Howl");
        Debug.Print($"{DateTime.Now.ToUniversalTime()}");
        RevitTask.Initialize(_app);
        RevitTask.RegisterGlobal(new ExecuteHuntEventHandler());
        await RevitTask.RunAsync(async () =>
        {
            foreach (var wolf in WolfQueue)
            {
                if (wolf is not IRevitWolf revitWolf) continue;
                await RevitTask.RaiseGlobal<ExecuteHuntEventHandler, IRevitWolf, IRevitWolfpack>(revitWolf);
            }

            HuntCompleted?.Invoke(this, new HuntCompletedEventArgs(true, this));
        });
    }

    public event EventHandler<HuntCompletedEventArgs>? HuntCompleted;
}