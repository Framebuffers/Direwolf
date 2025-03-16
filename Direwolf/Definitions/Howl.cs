using Direwolf.Contracts;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Direwolf.Definitions
{
    public record class Howl : IHowl
    {
        [JsonIgnore] public IWolf? Callback { get; set; }
        public void SendCatchToCallback(Prey c)
        {
            //var d = new Dictionary<string, object>()
            //{

            //    ["id"] = Guid.NewGuid(),
            //    ["createdAt"] = DateTime.UtcNow,
            //    ["wasCompleted"] = true,
            //    ["data"] = c
            //};
            Callback?.Catches.Push(c);
        }

        protected Stopwatch TimeTaken { get; set; } = new();

        public virtual bool Execute()
        {
            try
            {
                // A catch won't handle data retrieval on it's own, as it is just meant to be a dumb container.
                // Any data retrieval operation should be done here.
                // If, for example, the result returns a void or a bool itself (without having to get data itself)
                // Just return true. No need to forge a blank Catch. The Wolf *should* expect this result.
                if (Callback is not null)
                {
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
                { "callback", Callback?.GetType().Name ?? "unknown" },
                { "createdAt", DateTime.Now.ToString() }
            };
            return new Prey(d).ToString();
        }
    }
}
