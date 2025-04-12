using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Direwolf.Contracts;
using Direwolf.Primitives;
using IConnector = Direwolf.Contracts.IConnector;

namespace Direwolf;

/// <summary>
///     Direwolf creates wolves, taking a prototype Direwolf, attaching a Howls (an instruction) and itself as a callback.
///     Then, to dispatch wolves, it executes a function inside each Direwolf.
/// </summary>
public abstract class Direwolf : IHowler
{
    #region Properties

    [JsonIgnore] public List<IWolf> WolfQueue { get; } = [];

    #endregion

    #region Overrides

    public override string ToString()
    {
        return JsonSerializer.Serialize(ProcessedResults.Select(x => x.Value));
    }

    #endregion

    #region Hunting

    public Stopwatch TimeTaken { get; } = new();

    /// <summary>
    ///     Performs the query. Summons all the workers held in <see cref="WolfQueue" />,
    ///     executes the <see cref="Wolf.Hunt" /> method inside each other, and waits back for them to come back.
    ///     When the process is completed, the <see cref="HuntCompleted" /> event is invoked, signalling the
    ///     <see cref="Direwolf" /> that the process has been completed and that <see cref="Contracts.IConnector.Create" />
    ///     will be called.
    /// </summary>
    public abstract Task Howl();

    #endregion

    #region Constructors

    /// <summary>
    ///     Factory of <see cref="Wolf" /> workers. Takes a <see cref="IHowl" /> instruction, attaches it to the Summoner of a
    ///     given <see cref="Wolf" />, and enqueues the resulting Direwolf to the WolfQueue.
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="instruction"></param>
    protected Direwolf()
    {
    }

    public void CreateWolf(IHowl instruction, IConnector destination)
    {
        Wolf w = new(this, instruction, destination);
        w.Instruction.Wolf = w; // Join instruction with runner.
        WolfQueue.Add(w);
    }

    public void CreateWolf(IHowl instruction, List<IConnector> destinations)
    {
        foreach (var w in destinations.Select(destination => new Wolf(this, instruction, destination)))
        {
            w.Instruction.Wolf = w; // Join instruction with runner.
            WolfQueue.Add(w);
        }
    }

    public Dictionary<IConnector, List<IWolfpack>> ProcessedResults { get; set; } = [];

    #endregion
}