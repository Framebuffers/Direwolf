using Direwolf.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Direwolf.Definitions
{
    public abstract record class Howl : IHowl
    {
        [JsonIgnore] public IWolf? Callback { get; set; }
        public void SendCatchToCallback(Catch c) => Callback?.Catches.Push(c);
        public abstract bool Execute();
       
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
}
