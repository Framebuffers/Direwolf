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
            TaskDialog t = new("Inside Async");
            await RevitTask.RunAsync(() =>
            {
                t.MainContent = "Inside the function";
                t.Show();
                ExecuteQuery(dispatch, out _, queryName);
                //ShowResultToGUI();
                WriteToFile();
            });
        }

        public void WriteToFile()
        {
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wolfpack.json");
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
