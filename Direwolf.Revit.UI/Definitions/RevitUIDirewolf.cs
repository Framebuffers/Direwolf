using System.Diagnostics;
using Autodesk.Revit.UI;
using Direwolf.Contracts;
using Direwolf.Definitions;
using Direwolf.EventHandlers;
using Direwolf.Revit.Contracts;
using Direwolf.Revit.Howlers;
using Revit.Async;

namespace Direwolf.Revit.UI.Definitions;

public class RevitUIDirewolf : RevitDirewolf
{
    public new event EventHandler<HuntCompletedEventArgs>? HuntCompleted;
    private readonly ExternalCommandData _cmd;

    // Unlike an array, there's no need to check types before setting the document.
    public RevitUIDirewolf(IRevitHowl instruction, IConnector destination, ExternalCommandData cmd) : base(
        instruction, destination)
    {
        instruction.SetRevitDocument(cmd.Application.ActiveUIDocument.Document);
        WolfQueue.Enqueue(new Wolf { Instruction = instruction, Summoner = this });
        _cmd = cmd;
        _connector = destination;
        HuntCompleted += OnHuntCompleted;
    }
    
    // Converting from IHowl[] to IRevitHowl[] can cause runtime exceptions on write ops.
    // To get around this, a type check is done for each member of the array.
    public RevitUIDirewolf(IHowl[] instructions, IConnector destination, ExternalCommandData cmd) : base(
        instructions, destination)
    {
        foreach (var i in instructions)
        {
            if (i is IRevitHowl howl)
            {
                howl.SetRevitDocument(cmd.Application.ActiveUIDocument.Document);
                WolfQueue.Enqueue(new Wolf { Instruction = howl, Summoner = this });
            }
            else
            {
                throw new ArgumentException("Howl is not a RevitHowl");
            }
        }

        _cmd = cmd;
        _connector = destination;
        HuntCompleted += OnHuntCompleted;
    }

    public override async Task Howl()
    {
        try
        {
            RevitTask.Initialize(_cmd.Application);
            await RevitTask.RunAsync(() =>
            {
                TimeTaken.Start();
                foreach (var wolf in WolfQueue) wolf.Run();
                HuntCompleted?.Invoke(this, new HuntCompletedEventArgs(isSuccessful: true));
                TimeTaken.Stop();
            });
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
    }
}