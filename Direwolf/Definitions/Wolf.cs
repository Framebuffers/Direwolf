using System.Text.Json;
using System.Text.Json.Serialization;
using Direwolf.Contracts;

namespace Direwolf.Definitions;

/// <summary>
///     Inside a wolf there is two things: who summoned you, and what you need to do.
///     When the Howler calls Howls(), the Wolf attaches itself to the howl and executes the instruction inside the Howls.
/// </summary>
public record struct Wolf() : IWolf
{
    /// <summary>
    ///     Link to the summoner of this worker.
    /// </summary>
    [JsonIgnore]
    public IHowler? Callback { get; set; }

    /// <summary>
    ///     Query to be executed.
    /// </summary>
    [JsonIgnore]
    public IHowl? Instruction { get; set; }

    /// <summary>
    ///     Data obtained from a query.
    /// </summary>
    [JsonPropertyName("results")]
    public Stack<Prey> Catches { get; set; } = []; // this is a cache for results *for a particular Wolf*

    /// <summary>
    ///     Perform the task held inside <see cref="IHowl.Execute" />.
    /// </summary>
    /// <returns>True if task has been performed successfully, false if otherwise.</returns>
    public bool Run()
    {
        if (Instruction is not null)
            try
            {
                Instruction.Callback = this; // attach to load contents back the chain.
                Instruction.Execute();
                foreach (var c in Catches) Callback?.Den.Push(c);
                return true; // it did the thing!
            }
            catch (Exception e) // something went wrong.
            {
                Console.WriteLine(e.Message);
                return false;
            }

        return true; // nothing ran, so no error.
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(Catches);
    }
}