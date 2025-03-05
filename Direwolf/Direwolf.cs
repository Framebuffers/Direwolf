using Autodesk.Revit.UI;
using Direwolf.Contracts;
using Direwolf.Definitions;
using Revit.Async;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Direwolf
{
    public sealed class Direwolf
    {
        [JsonExtensionData]
        private Dictionary<string, Wolfpack> Queries { get; set; } = [];
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

        public async Task WriteAsyncQueries(IEnumerable<(IHowler, string)> queries)
        {
            //foreach
        }

        public void WriteToFile(string fn)
        {
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), $"{fn}.json");
            File.WriteAllText(fileName, JsonSerializer.Serialize(Queries));
        }


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
