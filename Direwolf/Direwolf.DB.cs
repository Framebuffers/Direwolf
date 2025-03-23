using Direwolf.Definitions;
using System;
using System.Collections.Generic;
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
    }
}
