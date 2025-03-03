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

        public void WriteToFile()
        {
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wolfpack.json");
            File.WriteAllText(fileName, JsonSerializer.Serialize(Queries));
        }

        public static void ShowResultToGUI(Wolfpack w)
        {
            //using StringWriter s = new();
            //foreach (var c in h.Den)
            //{
            //    s.WriteLine(c.ToString());
            //}

            TaskDialog t = new("Direwolf Query Results")
            {
                MainContent = $"ToString():\n{w}\n\nSerializer:\n{JsonSerializer.Serialize(w)}"
            };
            t.Show();
        }
    }
}
