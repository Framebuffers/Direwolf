using System.Diagnostics;
using System.Text.Json.Serialization;
using Direwolf.Contracts;

namespace Direwolf.Primitives;

public record Wolf : IWolf
{
    /// <summary>
    ///     Inside a wolf there is two things: who summoned you, and what you need to do.
    ///     When the Direwolf calls Howls(), the Direwolf attaches itself to the howl
    ///     and executes the instruction inside the Howls.
    /// </summary>
    public Wolf(IHowler callback, IHowl instruction, List<IConnector> destinations)
    {
        Summoner = callback;
        Instruction = instruction;
        Destinations = destinations;
    }

    public Wolf(IHowler callback, IHowl instruction, IConnector destination)
    {
        Summoner = callback;
        Instruction = instruction;
        Destinations.Add(destination);
    }

    [JsonIgnore] public IHowler Summoner { get; init; }
    [JsonIgnore] public IHowl Instruction { get; init; }
    [JsonIgnore] public IWolfpack? Result { get; set; }
    [JsonIgnore] public List<IConnector> Destinations { get; init; } = [];

    /// <summary>
    ///     Perform the task held inside <see cref="IHowl.ExecuteHunt" />.
    /// </summary>
    /// <returns>True if task has been performed successfully, false if otherwise.</returns>
    public void Hunt()
    {
        Debug.Print("Hunting started");
        if (Instruction is null || Summoner is null || Destinations is null)
            throw new NullReferenceException(); // nothing ran, so no error.
        Debug.Print("Inside Hunt loop");
        Result = Instruction.ExecuteHunt() ?? throw new NullReferenceException();
    }

    public void Deconstruct(out IHowler callback, out IHowl instruction)
    {
        callback = Summoner;
        instruction = Instruction;
    }

    // don't serialize this record: it's just a vessel for the Task being ran.
    public override string ToString()
    {
        return GetType().Name;
    }
}