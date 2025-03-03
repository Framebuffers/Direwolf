using Direwolf.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Direwolf.Definitions
{
     /// <summary>
    /// Howler creates wolves, taking a prototype Wolf, attaching a Howl (an instruction) and itself as a callback.
    /// Then, to dispatch wolves, it executes a function inside each Wolf.
    /// </summary>
    public record class Howler() : IHowler
    {
        [JsonPropertyName("Response")]
        public List<Catch> Den { get; set; } = [];

        [JsonIgnore]
        public List<IWolf> Wolfpack { get; set; } = [];

        public static IHowler CreateFailedQueryHowler(Exception e)
        {
            Howler h = new();
            Catch exception = new(new Dictionary<string, object>()
            {
                ["ExceptionRaised"] = new Dictionary<string, object>()
                {
                    ["Message"] = e.Message,
                    ["Source"] = e.Source,
                    ["TargetSite"] = e.TargetSite,
                    ["StackTrace"] = e.StackTrace
                }
            });
            h.Den.Add(exception);
            return h;
        }

        public void CreateWolf(IWolf runner, IHowl instruction) // wolf factory
        {
            runner.Instruction = instruction;
            runner.Callback = this;
            Wolfpack.Add(runner);
        }
        
        public virtual void Dispatch()
        {
            foreach (var wolf in Wolfpack)
            {
                wolf.Run();
            }
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(Den);
        }
    }
}
