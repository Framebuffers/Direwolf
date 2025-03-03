using Direwolf.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Direwolf.Definitions
{
    /// <summary>
    /// Inside a wolf there is two things: who summoned you, and what you need to do.
    /// When the Howler calls Run(), the Wolf attaches itself to the howl and executes the instruction inside the Howl.
    /// </summary>
    public abstract record class Wolf() : IWolf
    {
        [JsonIgnore] public IHowler? Callback { get; set; }
        [JsonIgnore] public IHowl? Instruction { get; set; }
        [JsonPropertyName("results")] public Stack<Catch> Catches { get; set; } = []; // this is a cache for results *for a particular Wolf*
        public abstract bool Run();
        public override string ToString()
        {
            var obj = new Dictionary<string, object>
            {
                ["Origin"] = Callback?.GetType().Name ?? "Direwolf",
                ["ServiceName"] = this.GetType().Name ?? "Lonewolf",
            };
            return new Catch(obj).ToString();
        }
    }

}
