using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Forest
{
    public class RecursiveLoad
    {
        public void Execute()
        {
            // Dispatch
            ExampleHowler howler = new();

            // Instruction
            ExampleHowl howl = new();
            
            // Messenger
            ExampleWolf wolf = new();
            howler.CreateWolf(wolf, howl); // attach instruction
            howler.Dispatch();

            foreach (var catches in howler.Den)
            {
                Console.WriteLine(catches.ToString());
            }
        }
    }

    // -------------------------------------------------
    public interface IHowler
    {
        public List<ICatches> Den { get; set; }
        public List<IWolf> Wolfpack { get; set; }
        public void CreateWolf(IWolf runner, IHowl instruction);
    }

    /// <summary>
    /// Howler creates wolves, taking a prototype Wolf, attaching a Howl (an instruction) and itself as a callback.
    /// Then, to dispatch wolves, it executes a function inside each Wolf.
    /// </summary>
    public record class ExampleHowler : IHowler
    {
        public List<ICatches> Den { get; set; } = [];
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
    }

    // -------------------------------------------------
    public interface IWolf
    {
        public IHowler? Callback { get; set; }
        public IHowl? Instruction { get; set; }
        public ICatches? Catches { get; set; }
        public bool Run();
    }

    /// <summary>
    /// Inside a wolf there is two things: who summoned you, and what you need to do.
    /// When the Howler calls Run(), the Wolf attaches itself to the howl and executes the instruction inside the Howl.
    /// </summary>
    public record class ExampleWolf() : IWolf
    {
        public IHowler? Callback { get; set; }
        public IHowl? Instruction { get; set; }

        public ICatches? Catches { get; set; } = new Catch();
        public bool Run()
        {
            if (Instruction is not null)
            {
                try
                {
                    Instruction.Callback = this; // attach to load contents back the chain.
                    if (Instruction.Execute())
                    {
                        if (Catches is not null) Callback?.Den.Add(Catches);
                        return true; // it did the thing!
                    }
                    else
                    {
                        return false; // there was an instruction, but it was not executed.
                    }
                }
                catch (Exception e) // something went wrong.
                { 
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            return true; // nothing ran, so no error.
        }
    }

    // -------------------------------------------------
    public interface IHowl
    {
        public IWolf? Callback { get; set; } // making it nullable sounds like a bad idea
        public bool Execute();
    }
    
    /// <summary>
    /// A Howl only has access to the Wolf who summoned it. It performs any operation.
    /// If, for example, the Howl has to get data to a Wolf (a Catch), it has to access it *through* the Callback.
    /// The Wolf will wait for its Howl to come back by waiting for the Execute() to return true.
    /// Else, the Wolf will handle the error.
    /// </summary>
    public record class ExampleHowl(): IHowl
    {
        public IWolf? Callback { get; set; }

        public bool Execute()
        {
            try
            {
                Console.WriteLine("Example Howl: attaching example wolf through a primary constructor");
                Console.WriteLine("Calling the execution *through the attached wolf*");
                Dictionary<string, string> s = [];
                s.Add("key", "123");

                // A catch won't handle data retrieval on it's own, as it is just meant to be a dumb container.
                // Any data retrieval operation should be done here.
                // If, for example, the result returns a void or a bool itself (without having to get data itself)
                // Just return true. No need to forge a blank Catch. The Wolf *should* expect this result.
                if (Callback is not null)
                {
                    Catch c = new(this, typeof(string), s);
                    Callback.Catches = c;
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
    }

    // -------------------------------------------------
    public interface ICatches
    {
        public Type ExpectedDataType { get; }
        public IHowl? Callback { get; }
    }

    /// <summary>
    /// A Catch is any collection of data. It holds a reference to the Howl who called it, the Type of data that it should retrieve, and a dictionary to hold data as string.
    /// The Type is captured as a way to sidestep the need to strongly type a dictionary, and to store data without recasting. It stores what it needs as raw strings, and knows the Type in case it needs to access something specific from it, just in case.
    /// </summary>
    /// <param name="Callback">The Howl where this Catch has to come back to.</param>
    /// <param name="ExpectedDataType">The Type of the data held inside the Results dictionary</param>
    /// <param name="Results">Results as raw string KVP's</param>
    public record struct Catch(IHowl Callback, Type ExpectedDataType, Dictionary<string, string> Results) : ICatches
    {
        
    }
}
