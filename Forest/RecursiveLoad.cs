using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Forest
{
    public class RecursiveLoad
    {
        public static void Execute()
        {
            // Dispatch
            ExampleHowler howler = new();

            // Instruction
            ExampleHowl howl = new();
            
            // Messenger
            ExampleWolf wolf = new();
            howler.CreateWolf(wolf, howl); // attach instruction

            //foreach (var w in howler.Wolfpack)
            //{
            //    Console.WriteLine(w);
            //}
            howler.Dispatch();

            var opt = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            
            
            Console.WriteLine(JsonSerializer.Serialize(howler, opt));
            //foreach (var catches in howler.Den)
            //{
            //    Console.WriteLine(howler.ToString());
            //}
        }
    }

    // -------------------------------------------------
    public interface IHowler
    {
        public List<Catch> Den { get; set; }
        public List<IWolf> Wolfpack { get; set; }
        public void CreateWolf(IWolf runner, IHowl instruction);
    }

    /// <summary>
    /// Howler creates wolves, taking a prototype Wolf, attaching a Howl (an instruction) and itself as a callback.
    /// Then, to dispatch wolves, it executes a function inside each Wolf.
    /// </summary>
    public record class ExampleHowler : IHowler
    {
        [JsonPropertyName("Response")]
        public List<Catch> Den { get; set; } = [];

        [JsonIgnore]
        public List<IWolf> Wolfpack { get; set; } = [];

        public void CreateWolf(IWolf runner, IHowl instruction) // wolf factory
        {
            runner.Instruction = instruction;
            runner.Callback = this;
            Wolfpack.Add(runner);
        }
        
        public void Dispatch()
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

    // -------------------------------------------------
    public interface IWolf
    {
        public IHowler? Callback { get; set; }
        public IHowl? Instruction { get; set; }
        public Stack<Catch> Catches { get; set; }
        public bool Run();
    }

    /// <summary>
    /// Inside a wolf there is two things: who summoned you, and what you need to do.
    /// When the Howler calls Run(), the Wolf attaches itself to the howl and executes the instruction inside the Howl.
    /// </summary>
    public record struct ExampleWolf() : IWolf
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
                    if (Callback is null) Console.WriteLine($"Callback is null");
                    Instruction.Execute();
                    
                    if (Catches is not null) Callback?.Den.AddRange(Catches);
                    //Console.WriteLine(this);
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
            var obj = new Dictionary<string, object>
            {
                ["Origin"] = Callback?.GetType().Name ?? "Direwolf",
                ["ServiceName"] = this.GetType().Name ?? "Lonewolf",
            };
            return new Catch(obj).ToString();
        }
    }

    // -------------------------------------------------
    public interface IHowl
    {
        public IWolf? Callback { get; set; } // making it nullable sounds like a bad idea
        public bool Execute();
        public void SendCatchToCallback(Catch c);
    }
    
    /// <summary>
    /// A Howl only has access to the Wolf who summoned it. It performs any operation.
    /// If, for example, the Howl has to get data to a Wolf (a Catch), it has to access it *through* the Callback.
    /// The Wolf will wait for its Howl to come back by waiting for the Execute() to return true.
    /// Else, the Wolf will handle the error.
    /// </summary>
    public record class ExampleHowl(): IHowl
    {
        [JsonIgnore] public IWolf? Callback { get; set; }

        public void SendCatchToCallback(Catch c) => Callback?.Catches.Push(c);

        public bool Execute()
        {
            try
            {
                // A catch won't handle data retrieval on it's own, as it is just meant to be a dumb container.
                // Any data retrieval operation should be done here.
                // If, for example, the result returns a void or a bool itself (without having to get data itself)
                // Just return true. No need to forge a blank Catch. The Wolf *should* expect this result.
                if (Callback is not null)
                {
                    Guid i = Guid.NewGuid();
                    var s = new Dictionary<string, object>()
                    {
                        ["Thing1"] = GetType().FullName ?? string.Empty,
                        ["Information"] = new string[] {"info1", "info2", $"{GetType().AssemblyQualifiedName}"}
                    };

                    var s2 = new Dictionary<string, object>()
                    {
                        ["NewInformation"] = GetType().MemberType,
                        ["AssemblyInformation"] = new string[] { GetType().Namespace, GetType().ToString() },
                        ["test"] = i
                    };

                    var d = new Dictionary<string, object>()
                    {
                        ["Data"] = new List<Dictionary<string, object>>() { s, s2 }
                    };

                    SendCatchToCallback(new Catch(d));
                    return true; // hunt successful!
                }
                else
                {
                    return false; // howled into the air, but no wolf to hear back...
                }
            }
            catch
            {
                return false; // failed to hunt.
            }
        }

        public override string ToString() // the default implementation will recursively serialize everything up the tree. that is: not good.
        {
            var d = new Dictionary<string, object>()
            {
                { "Callback", Callback?.GetType().Name ?? "unknown" },
                { "Timestamp", DateTime.Now.ToString() }
            };
            return new Catch(d).ToString();
        }
    }

    // -------------------------------------------------
    public readonly record struct Catch([property: JsonExtensionData] Dictionary<string, object> Result)
    {
        public override string ToString()
        {
            return JsonSerializer.Serialize(Result).ToString();
        }
    }
}
