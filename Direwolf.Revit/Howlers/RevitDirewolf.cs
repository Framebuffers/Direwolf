using Direwolf.Contracts;
using Direwolf.Definitions;
using Direwolf.Revit.Contracts;

namespace Direwolf.Revit.Howlers;

/// <summary>
///     Exactly the same as a regular Direwolf, except that it checks if the Howl implements IDynamicRevitHowl.
/// </summary>
public partial class RevitDirewolf : Direwolf
{
    /// <summary>
    ///     Factory of <see cref="Wolf" /> workers. Takes a <see cref="IHowl" /> instruction, attaches it to the Summoner of a
    ///     given <see cref="Wolf" />, and enqueues the resulting Direwolf to the WolfQueue.
    /// </summary>
    /// <param name="instruction"></param>
    /// <param name="destination"></param>
    protected RevitDirewolf(IHowl instruction, IConnector destination) : base(instruction, destination)
    {
        if (instruction is not IRevitHowl) throw new ArgumentException("The instruction is not a RevitHowl");
        WolfQueue.Enqueue(new Wolf { Instruction = instruction, Summoner = this });
        _connector = destination;
        HuntCompleted += OnHuntCompleted;
    }
    
    public static RevitDirewolf CreateInstance(IHowl instruction, IConnector destination)
    {
        return new RevitDirewolf(instruction, destination);
    }
}