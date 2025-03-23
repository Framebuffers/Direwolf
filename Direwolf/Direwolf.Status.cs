using Direwolf.Contracts;
using Direwolf.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Direwolf
{
    public partial class Direwolf
    {
        public string GetQueueInfo()
        {
            try
            {
                var howlers = new Dictionary<string, object>
                {
                    ["count"] = Howlers.Count.ToString() ?? ""
                };

                foreach (IHowler h in Howlers)
                {
                    var howler = new Dictionary<string, object>();
                    var hw = new Dictionary<string, object>
                    {
                        ["wolves"] = h.Wolfpack.Count,
                        ["catchCount"] = h.Results.Count
                    };

                    howler["name"] = h.GetType().Name;
                    howler["metadata"] = hw;
                    howlers["howler"] = howler;
                }

                var total = new Dictionary<string, object>
                {
                    ["totalHowlers"] = howlers
                };
                return JsonSerializer.Serialize(new Prey(total));
            }
            catch
            {
                return "";
            }
        }

    }
}
