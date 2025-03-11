using Direwolf.Contracts;
using Direwolf.Definitions;
using System.Text.Json.Serialization;
using System.Text.Json;
using Direwolf.Revit.Contracts;
using Direwolf.EventHandlers;

namespace Direwolf.Revit.Howlers
{
    /// <summary>
    /// Exactly the same as a regular Howler, except that it checks if the Howl implements IDynamicRevitHowl.
    /// A bit of a hack but works.
    /// </summary>
    public record class RevitHowler : IHowler
    {
        public event EventHandler<HuntCompletedEventArgs>? HuntCompleted;
        [JsonPropertyName("Response")] public Stack<Prey> Den { get; set; } = [];
        [JsonIgnore] public Queue<IWolf> Wolfpack { get; set; } = [];

        public virtual void CreateWolf(IWolf runner, IHowl instruction) // wolf factory
        {
            if (instruction is IRevitHowl)
            {
                runner.Instruction = instruction;
                runner.Callback = this;
                Wolfpack.Enqueue(runner);
            }
            else
            {
                throw new ArgumentException("Howl is not a valid Revit Howl.");
            }
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

