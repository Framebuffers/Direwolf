using System.Text.Json.Serialization;
using System.Text.Json;
using Direwolf.Definitions.Dynamics;
using Direwolf.Contracts.Dynamics;
using Direwolf.Revit.Contracts.Dynamics;

namespace Direwolf.Revit.Howlers.Dynamics
{
    /// <summary>
    /// Exactly the same as a regular Howler, except that it checks if the Howl implements IDynamicRevitHowl.
    /// A bit of a hack but works.
    /// </summary>
    public record class DynamicRevitHowler : IDynamicHowler
    {
        [JsonPropertyName("Response")]
        public Stack<DynamicCatch> Den { get; set; } = [];

        [JsonIgnore]
        public Queue<IDynamicWolf> Wolfpack { get; set; } = [];
        public virtual void CreateWolf(IDynamicWolf runner, IDynamicHowl instruction) // wolf factory
        {
            if (instruction is IDynamicRevitHowl)
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

        public DynamicWolfpack Howl()
        {
            try
            {
                foreach (var wolf in Wolfpack)
                {
                    wolf.Run();
                }
                return new DynamicWolfpack(this, GetType().Name);
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
