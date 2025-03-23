using Direwolf.Definitions;
using Direwolf.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Direwolf
{
    public partial class Direwolf
    {
        // <summary>
        /// This is a proof of concept, not a production-ready solution. Please **CHANGE THIS** if you plan to deploy.
        /// </summary>
        private static readonly DbConnectionString _default = new("localhost", 5432, "wolf", "awoo", "direwolf");
        [JsonExtensionData] private Wolfden Queries { get; set; } = new(_default);
        public async void SendAllToDB()
        {
            try
            {
                await Queries.Send();
            }
            catch (Exception e)
            {
                $"{e.Message}".ToConsole();
            }
        }
        public virtual void SendAllToScreen()
        {
            using StringWriter s = new();
            foreach (var q in Queries)
            {
                s.Write(q.ToString());
            }
            s.ToString().ToConsole();
            s.Dispose();
        }
    }
}
