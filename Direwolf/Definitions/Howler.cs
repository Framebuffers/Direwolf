using Direwolf.Contracts;
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
        [JsonPropertyName("Response")]
        public Stack<Prey> Den { get; set; } = [];

        [JsonIgnore]
        public Queue<IWolf> Wolfpack { get; set; } = [];
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

                return new Wolfpack(this, GetType().Name);
            }
            catch
            {
                throw new ApplicationException();
            }
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(Den);
        }
    }
}
