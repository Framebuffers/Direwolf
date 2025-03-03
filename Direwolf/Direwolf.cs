using Autodesk.Internal.Windows;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
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
        private List<Wolfpack> Results { get; set; } = [];
        public void ExecuteQuery(IHowler dispatch, IHowl instruction, IWolf? runner = null, string queryName = "Query")
        {
            try
            {
                var howler = dispatch ?? new Howler();
                var wolf = runner ?? new Wolf();
                howler.CreateWolf(wolf, instruction);
                howler.Dispatch();
                Results.Add(new Wolfpack(howler, queryName));
            }
            catch (Exception e)
            {
                Results.Add(new Wolfpack(Howler.CreateFailedQueryHowler(e), "FailedQuery"));
            }
        }

        public async void ExecuteRevitQueryAsync(ExternalCommandData cmd, IHowler dispatcher, IHowl instruction, string queryName = "", IWolf? runner = null)
        {
            RevitTask.Initialize(cmd.Application);
            var t = await RevitTask.RunAsync(
                () =>
                {
                    ExecuteQuery(dispatcher, instruction, runner, queryName);
                    return 0;
                });
        }

        public void ShowResultToGUI()
        {
            TaskDialog t = new("Direwolf Query Results")
            {
                MainContent = GetResultsAsJson(),
            };
            t.Show();
        }

        public string GetResultsAsJson() => JsonSerializer.Serialize(Results);
    }
}
