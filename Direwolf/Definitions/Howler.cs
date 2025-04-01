using Direwolf.Contracts;
using Direwolf.EventHandlers;
using System.Diagnostics;
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
        /// <summary>
        /// Sends a signal to <see cref="Direwolf"/> that the process has been completed, to pass control to the serialization routines.
        /// </summary>
        public event EventHandler<HuntCompletedEventArgs>? HuntCompleted;

        /// <summary>
        /// Raw information obtained from a query. The results in <see cref="Wolfpack.Results"/> is the serialized content of Den.
        /// </summary>
        [JsonPropertyName("response")] public Stack<Prey> Den { get; set; } = [];

        /// <summary>
        /// Queue of workers to be deployed.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore] public Queue<IWolf> WolfQueue { get; set; } = [];

        /// <summary>
        /// Factory of <see cref="IWolf"/> workers. Takes a <see cref="IHowl"/> instruction, attaches it to the Callback of a given <see cref="IWolf"/>, and enqueues the resulting Wolf to the WolfQueue.
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="instruction"></param>
        public virtual void CreateWolf(IWolf runner, IHowl instruction) // wolf factory
        {
            runner.Instruction = instruction;
            runner.Callback = this;
            WolfQueue.Enqueue(runner);
        }

        /// <summary>
        /// Performs the query. Summons all the workers held in <see cref="WolfQueue"/>, executes the <see cref="IWolf.Run"/> method inside each other, and waits back for them to come back.
        /// When the process is completed, the <see cref="HuntCompleted"/> event is invoked, signalling the <see cref="Direwolf"/> that the process has been completed and that a Wolfpack has been generated.
        /// </summary>
        /// <param name="testName">Name of the query to perform</param>
        /// <returns>A record containing metadata about the query with the results serialized inside</returns>
        /// <exception cref="ApplicationException">Thrown whenever a worker has encountered an issue and the execution returns false.</exception>
        public virtual Wolfpack Howl(string testName)
        {
            try
            {
                Stopwatch s = new();
                s.Start();
                foreach (var wolf in WolfQueue)
                {
                    wolf.Run();
                }

                HuntCompleted?.Invoke(this, new HuntCompletedEventArgs() { IsSuccessful = true });
                s.Stop();
                return new Wolfpack(this, "", "", "", true, s.Elapsed.TotalSeconds)
                {
                    TestName = testName
                };
            }
            catch
            {
                HuntCompleted?.Invoke(this, new HuntCompletedEventArgs() { IsSuccessful = false });
                throw new ApplicationException();
            }
        }

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(Den);
        }

    }
}
