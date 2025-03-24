using Direwolf.Contracts;
using Direwolf.EventHandlers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Direwolf.Definitions
{
    /// <summary>
    /// Howler creates wolves, taking a prototype Wolf, attaching a Howls (an instruction) and itself as a callback.
    /// Then, to dispatch wolves, it executes a function inside each Wolf.
    /// </summary>
    public abstract record class Howler : IHowler
    {
        public event EventHandler<HuntCompletedEventArgs>? HuntCompleted;

        [JsonPropertyName("response")] public Stack<Prey> Results { get; set; } = [];
        [JsonIgnore] public Queue<IWolf> Wolfpack { get; set; } = [];
        public abstract WolfpackTarget FinalTarget { get; set; }
        public abstract Wolfpack Howl(string testName);

        public virtual void CreateWolf(IWolf runner, IHowl instruction, WolfpackTarget where) // wolf factory
        {
            runner.Instruction = instruction;
            runner.Callback = this;
            Wolfpack.Enqueue(runner);
            FinalTarget = where;
        }
        public override string ToString()
        {
            return JsonSerializer.Serialize(Results);
        }
    }
}
