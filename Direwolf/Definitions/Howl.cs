using Direwolf.Contracts;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System;

namespace Direwolf.Definitions
{
    public abstract record class Howl : IHowl
    {
        [JsonIgnore] public IWolf? Callback { get; set; }
        protected Stopwatch TimeTaken { get; set; } = new();
        public void SendCatchToCallback(Prey c)
        {
            Callback?.Catches.Push(c);
        }
        public abstract bool Execute();
        public override string ToString() // the default implementation will recursively serialize everything up the tree. that is: not good.
        {
            var d = new Dictionary<string, object>()
            {
                { "callback", Callback?.GetType().Name ?? "unknown" },
                { "createdAt", DateTime.Now.ToString() }
            };
            return new Prey(d).ToString();
        }
    }
}
