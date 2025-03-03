using Autodesk.Internal.Windows;
using Autodesk.Revit.DB.Mechanical;
using Direwolf.Contracts;
using Direwolf.Definitions;
using Revit.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Direwolf
{
    public sealed class Direwolf
    {
        //private JsonDocument Wolfden { get; set; } = [];
        //public string GetCollectedData()
        //{

        //}

        //public void LoadCollectedData(IHowler howler)
        //{
        //    if (howler is not null)
        //    {
        //        JsonObject j = JsonSerializer.Serialize(howler); 

        //    }
        //}

        private List<Catch> Wolfden { get; set; } = [];
        public string GetData() => JsonSerializer.Serialize(Wolfden);
        public async Task AsyncFetch(IHowler wolfpack)
        {
            await RevitTask.RunAsync(() =>
            {
                wolfpack.Dispatch();
                var r = new Dictionary<string, object>()
                {
                    [wolfpack.GetType().Name] = wolfpack.ToString() ?? string.Empty
                };
                Wolfden.Add(new Catch(r));
            });
        }
    }
}
