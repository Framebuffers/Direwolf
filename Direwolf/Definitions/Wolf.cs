using Direwolf.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Direwolf.Definitions
{
    /// <summary>
    /// Inside a wolf there is two things: who summoned you, and what you need to do.
    /// When the Howler calls Howls(), the Wolf attaches itself to the howl and executes the instruction inside the Howls.
    /// </summary>
    public record struct Wolf() : IWolf
    {
        [JsonIgnore] public IHowler? Callback { get; set; }
        [JsonIgnore] public IHowl? Instruction { get; set; }
        [JsonPropertyName("Results")] public Stack<Catch> Catches { get; set; } = []; // this is a cache for results *for a particular Wolf*
        public bool Run()
        {
            if (Instruction is not null)
            {
                try
                {
                    Instruction.Callback = this; // attach to load contents back the chain.
                    //if (Callback is null) Console.WriteLine($"Callback is null");
                    Instruction.Execute();
                    foreach (var c in Catches)
                    {
                        Callback?.Den.Push(c);
                    }
                    return true; // it did the thing!
                }
                catch (Exception e) // something went wrong.
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            return true; // nothing ran, so no error.
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(Catches);
            //var obj = new Dictionary<string, object>
            //{
            //    ["Origin"] = Callback?.GetType().Name ?? "Direwolf",
            //    ["ServiceName"] = this.GetType().Name ?? "Lonewolf",
            //};
            //return new Catch(obj).ToString();
        }
    }
}