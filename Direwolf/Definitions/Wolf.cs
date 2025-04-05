using System.Text.Json.Serialization;
using Direwolf.Contracts;

namespace Direwolf.Definitions;

/// <summary>
///     Inside a wolf there is two things: who summoned you, and what you need to do.
///     When the Direwolf calls Howls(), the Direwolf attaches itself to the howl and executes the instruction inside the
///     Howls.
/// </summary>
public readonly record struct Wolf([property: JsonIgnore] IHowler? Callback, [property: JsonIgnore] IHowl? Instruction)
{
    /// <summary>
    ///     Perform the task held inside <see cref="IHowl.Execute" />.
    /// </summary>
    /// <returns>True if task has been performed successfully, false if otherwise.</returns>
    public bool Run()
    {
        if (Instruction is null) return false; // nothing ran, so no error.
        try
        {
            Instruction.Callback = this; // attach to load contents back the chain.
            Instruction.Execute();
            return true; // it did the thing!
        }
        catch (Exception e) // something went wrong.
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }
}