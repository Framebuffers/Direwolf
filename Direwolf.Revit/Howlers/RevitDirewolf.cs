using Direwolf.Contracts;
using Direwolf.Definitions;
using Direwolf.Revit.Contracts;

namespace Direwolf.Revit.Howlers;

/// <summary>
///     Exactly the same as a regular Direwolf, except that it checks if the Howl implements IDynamicRevitHowl.
/// </summary>
public class RevitDirewolf : Direwolf
{
    /// <summary>
    ///     Factory of <see cref="Wolf" /> workers. Takes a <see cref="IHowl" /> instruction, attaches it to the Callback of a
    ///     given <see cref="Wolf" />, and enqueues the resulting Direwolf to the WolfQueue.
    /// </summary>
    /// <param name="instruction"></param>
    /// <param name="destination"></param>
    protected RevitDirewolf(IHowl instruction, IDirewolfConnector destination) : base(instruction, destination)
    {
        if (instruction is not IRevitHowl) throw new ArgumentException("The instruction is not a RevitHowl");
        WolfQueue.Enqueue(new Wolf { Instruction = instruction, Callback = this });
        _connector = destination;
        HuntCompleted += OnHuntCompleted;
    }

/// <summary>
/// <inheritdoc cref="Direwolf"/>
/// </summary>
/// <param name="instructions"></param>
/// <param name="destination"></param>
    protected RevitDirewolf(IHowl[] instructions, IDirewolfConnector destination) : base(instructions, destination)
    {
        foreach (var i in instructions)
        {
            WolfQueue.Enqueue(new Wolf { Instruction = i, Callback = this });
        }

        _connector = destination;
        HuntCompleted += OnHuntCompleted;
    }
}