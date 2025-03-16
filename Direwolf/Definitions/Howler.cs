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
    //[JsonSerializable(typeof(Howler))] 
    public record class Howler : IHowler
    {
        public event EventHandler<HuntCompletedEventArgs>? HuntCompleted;

        [JsonPropertyName("response")] public Stack<Prey> Den { get; set; } = [];
        [JsonIgnore] public Queue<IWolf> Wolfpack { get; set; } = [];

        public virtual void CreateWolf(IWolf runner, IHowl instruction) // wolf factory
        {
            runner.Instruction = instruction;
            runner.Callback = this;
            Wolfpack.Enqueue(runner);
        }
        
        public Wolfpack Howl()
        {
            try
            {
                foreach (var wolf in Wolfpack)
                {
                    wolf.Run();
                }

                HuntCompleted?.Invoke(this, new HuntCompletedEventArgs() { IsSuccessful = true});
                return new Wolfpack(this, GetType().Name);
            }
            catch
            {
                HuntCompleted?.Invoke(this, new HuntCompletedEventArgs() { IsSuccessful = false});
                throw new ApplicationException();
            }
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(Den);
        }
    }
}
