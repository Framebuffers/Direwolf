using System.Text.Json.Serialization;
using Direwolf.Contracts;

namespace Direwolf.Definitions;

/// <summary>
///     Inside a wolf there are two things: who summoned you, and what you need to do.
///     When the Direwolf calls Howl(), the Direwolf attaches itself to the howl and executes the instruction inside the
///     Howls.
/// </summary>
public readonly record struct Wolf : IWolf
{
    /// <summary>
    ///     Inside a wolf there is two things: who summoned you, and what you need to do.
    ///     When the Direwolf calls Howls(), the Direwolf attaches itself to the howl and executes the instruction inside the
    ///     Howls.
    /// </summary>
    public Wolf(IHowler callback, IHowl instruction)
    {
        Summoner = callback;
        Instruction = instruction;
    }

    /// <summary>
    ///     Perform the task held inside <see cref="IHowl.Hunt" />.
    /// </summary>
    /// <returns>True if task has been performed successfully, false if otherwise.</returns>
    public bool Run()
    {
        if (Instruction is null) return false; // nothing ran, so no error.
        try
        {
            Instruction.Wolf = this; // attach to load contents back the chain.
            Instruction.Hunt();
            return true; // it did the thing!
        }
        catch (Exception e) // something went wrong.
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }

    [JsonIgnore] public IHowler Summoner { get; init; }
    [JsonIgnore] public IHowl Instruction { get; init; }

    public void Deconstruct(out IHowler Callback, out IHowl Instruction)
    {
        Callback = this.Summoner;
        Instruction = this.Instruction;
    }
}