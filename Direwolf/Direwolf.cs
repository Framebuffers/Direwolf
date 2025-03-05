using Autodesk.Revit.UI;
using Direwolf.Contracts;
using Direwolf.Contracts.Dynamics;
using Direwolf.Definitions;
using Direwolf.Definitions.Dynamics;
using Direwolf.EventHandlers;
using IronPython.Compiler.Ast;
using Revit.Async;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Direwolf
{
    public sealed class Direwolf
    {
        [JsonExtensionData]
        public Dictionary<string, Wolfpack> Queries { get; set; } = [];

        [JsonExtensionData]
        public Dictionary<string, DynamicWolfpack> DynamicQueries { get; set; } = [];

        public void ExecuteQuery(IHowler dispatch, out Wolfpack result, string queryName = "Query")
        {
            try
            {
                result = dispatch.Howl();
                Queries.Add(queryName, result);
            }
            catch
            {
                throw new Exception();
            }
        }
        public async void ExecuteQueryAsync(IHowler dispatch, string queryName = "Query")
        {
            await RevitTask.RunAsync(() =>
            {
                ExecuteQuery(dispatch, out _, queryName);
                WriteToFile(queryName);
            });
        }

        public void DynamicExecuteQuery(IDynamicHowler dispatch, out DynamicWolfpack result, string queryName = "Query")
        {
            try
            {
                result = dispatch.Howl();
                DynamicQueries.Add(queryName, result);
            }
            catch
            {
                throw new Exception();
            }
        }

        //public async void DynamicExecuteQueryAsync(IDynamicHowler dispatch, string queryName = "Query")
        //{
        //    await RevitTask.RunAsync(() =>
        //    {
        //        DynamicExecuteQuery(dispatch, out _, queryName);
        //        DynamicWriteToFile(queryName);
        //    });
        //}

        public async void ExecuteDynamicAsyncHunt(IDynamicHowler dispatch, string queryName = "Query")
        {
            DynamicWolfpack result = await RevitTask.RunAsync(
                () =>
                {
                    try
                    {
                       return dispatch.Howl();
                    }
                    catch { throw new Exception("Cannot dispatch Wolves"); }
                });

            // go hunting
            DynamicQueries.Add(queryName, result);

            // is it done?
            bool hasBeenSerialized = await RevitTask.RaiseGlobal<DirewolfDynHuntExternalEventHandler, DynamicWolfpack, bool>(result);

            // if so, write a file
            if (hasBeenSerialized)
            {
                WriteToFile($"{queryName}.json");
            }
        }

        public void WriteToFile(string fn)
        {
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), $"{fn}.json");
            File.WriteAllText(fileName, JsonSerializer.Serialize(Queries));
        }

        //public void DynamicWriteToFile(string fn)
        //{
        //    string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), $"{fn}.json");
        //    File.WriteAllText(fileName, JsonSerializer.Serialize(DynamicQueries));
        //}

        public void ShowResultToGUI()
        {
            using StringWriter s = new();
            foreach(var result in Queries)
            {
                s.WriteLine(result.Value.ToString());
            }

            TaskDialog t = new("Direwolf Query Results")
            {
                MainContent = s.ToString()
            };
            t.Show();
        }
    }
}
