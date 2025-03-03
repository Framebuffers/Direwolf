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
        //private List<Wolfpack> Results { get; set; } = [];
        //private List<Catch> Catches { get; set; } = [];
        public static Wolfpack ExecuteQuery(IHowler dispatch, IHowl instruction, IWolf? runner = null, string queryName = "Query")
        {
            try
            {
                var wolf = runner ?? new Wolf();
                dispatch.CreateWolf(wolf, instruction);
                //var r = dispatch.Howl()
                //dispatch.Dispatch();
                return dispatch.Howl();
                //return JsonSerializer.Serialize(dispatch.ToString());
                //Catches.AddRange(dispatch.Den);
            }
            catch
            {
                throw new Exception();
                //return Howler.CreateFailedQueryHowler(e);
            }
        }

        public static void WriteToFile(Wolfpack w)
        {
            File.WriteAllText("""%HOMEDRIVE%HOMEPATH\Desktop\wolfpack.json""", w.ToString());
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

        //public string GetResultsAsJson() => JsonSerializer.Serialize(res);
    }
}
