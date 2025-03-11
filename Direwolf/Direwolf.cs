using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.UI;
using Direwolf.Contracts;
using Direwolf.Contracts.Dynamics;
using Direwolf.Definitions;
using Direwolf.Definitions.Dynamics;
using Direwolf.EventHandlers;
using IronPython.Compiler.Ast;
using Revit.Async;
using System.CodeDom;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

namespace Direwolf
{
    public class Direwolf
    {
        private readonly UIApplication? _app;
        public Prey[] Database
        {
            get
            {
                Stack<Prey> results = [];
                try
                {
                    ICollection<Element> allValidElements = new FilteredElementCollector(_app?.ActiveUIDocument.Document)
                        .WhereElementIsNotElementType()
                        .WhereElementIsViewIndependent()
                        .ToElements();

                    var elementsSortedByFamily = new Dictionary<string, object>();

                    foreach ((Element e, string familyName) in from Element e in allValidElements
                                                               let f = e as FamilyInstance
                                                               where f is not null
                                                               let familyName = f.Symbol.Family.Name
                                                               select (e, familyName))
                    {
                        if (!elementsSortedByFamily.TryGetValue(familyName, out dynamic? value))
                        { 
                            value = new ExpandoObject();
                            elementsSortedByFamily[familyName] = value;
                        }
                        value.Add(e);
                    }
                    results.Push(new Prey(elementsSortedByFamily));
                }
                catch
                {
                    throw new Exception("Could not get DB snapshot.");
                }
                return [.. results];
            }
        }


        public IEnumerable<Element> GetElementsFromFamily(string familyName) => Database
            .SelectMany(e => e.Result
            .Where(r => r.Key
            .Equals(familyName, StringComparison.Ordinal))
            .SelectMany(r => (List<Element>)r.Value));

        public Direwolf(UIApplication app) { _app = app; }
        public Direwolf(IHowler howler, UIApplication app)
        {
            Howlers.Enqueue(howler);
            _app = app;
        }
        public Direwolf(IDynamicHowler howler, UIApplication app)
        {
            DynamicHowlers.Enqueue(howler);
            _app = app;
        }
        public Direwolf(IHowler howler, IHowl instructions, UIApplication app)
        {
            howler.CreateWolf(new Wolf(), instructions);
            Howlers.Enqueue(howler);
            _app = app;
        }

        public Direwolf(IDynamicHowler howler, IDynamicHowl instructions, UIApplication app)
        {
            howler.CreateWolf(new DynamicWolf(), instructions);
            DynamicHowlers.Enqueue(howler);
            _app = app;
        }

        public Direwolf(IHowler howler, IHowl instructions, IWolf wolf, UIApplication app)
        {
            howler.CreateWolf(wolf, instructions);
            Howlers.Enqueue(howler);
            _app = app;
        }

        public Direwolf(IDynamicHowler howler, IDynamicHowl instructions, IDynamicWolf wolf, UIApplication app)
        {
            howler.CreateWolf(wolf, instructions);
            DynamicHowlers.Enqueue(howler);
            _app = app;
        }

        public void QueueHowler(IHowler howler)
        {
            ArgumentNullException.ThrowIfNull(howler);
            Howlers.Enqueue(howler);
        }

        public void QueueHowler(IDynamicHowler howler)
        {
            ArgumentNullException.ThrowIfNull(howler);
            DynamicHowlers.Enqueue(howler);
        }

        private Queue<IHowler> Howlers { get; set; } = [];
        private Queue<IDynamicHowler> DynamicHowlers { get; set; } = [];
        public string GetQueueInfo()
        {
            try
            {
                dynamic howlers = new ExpandoObject();
                howlers.Count = Howlers.Count.ToString() ?? "";

                foreach (IHowler h in Howlers)
                {
                    dynamic howler = new ExpandoObject();
                    dynamic hw = new ExpandoObject();
                    hw.Wolves = h.Wolfpack.Count;
                    hw.CatchCount = h.Den.Count;
                    howler.Name = h.GetType().Name;
                    howler.Metadata = hw;
                    howlers.Howler = howler;
                }

                dynamic dynHowlers = new ExpandoObject();
                dynHowlers.Count = DynamicHowlers.Count;

                foreach (IDynamicHowler h in DynamicHowlers)
                {
                    dynamic howler = new ExpandoObject();
                    dynamic hw = new ExpandoObject();
                    hw.Wolves = h.Wolfpack.Count;
                    hw.CatchCount = h.Den.Count;
                    howler.Name = h.GetType().Name;
                    howler.Metadata = hw;
                    howlers.Howler = howler;
                }

                dynamic total = new ExpandoObject();
                total.TotalHowlers = howlers;
                total.TotalDynamicHowlers = dynHowlers;

                return JsonSerializer.Serialize(new DynamicCatch(total));
            }
            catch
            {
                return "";
            }
        }

        [JsonExtensionData]
        public Dictionary<string, Wolfpack> Queries { get; set; } = [];

        [JsonExtensionData]
        public Dictionary<string, DynamicWolfpack> DynamicQueries { get; set; } = [];

        public string GetQueryInfo()
        {
            dynamic queries = new ExpandoObject();
            queries.Count = Queries.Count;

            dynamic queryInfo = new ExpandoObject();
            foreach (var query in Queries)
            {
                dynamic queryData = new ExpandoObject();
                queryData.Name = query.Key;
                dynamic wolfpack = new ExpandoObject();
                wolfpack.DateTime = query.Value.Timestamp;
                wolfpack.GUID = query.Value.GUID;
                wolfpack.ResultCount = query.Value.ResultCount;
                queryData.Wolfpack = wolfpack;
                queryInfo.Query = queryData;
            }

            dynamic dynQueries = new ExpandoObject();
            queries.Count = DynamicQueries.Count;

            dynamic dynQueryInfo = new ExpandoObject();
            foreach (var query in DynamicQueries)
            {
                dynamic queryData = new ExpandoObject();
                queryData.Name = query.Key;
                dynamic wolfpack = new ExpandoObject();
                wolfpack.DateTime = query.Value.Timestamp;
                wolfpack.GUID = query.Value.GUID;
                wolfpack.ResultCount = query.Value.ResultCount;
                queryData.Wolfpack = wolfpack;
                queryInfo.Query = queryData;
            }

            dynamic totals = new ExpandoObject();
            totals.Queries = queries;
            totals.DynamicQueries = dynQueries;
            totals.TotalQueries = Queries.Count + DynamicQueries.Count;

            return JsonSerializer.Serialize(new DynamicCatch(totals));
        }

        public void Hunt(string queryName = "query")
        {
            try
            {
                if (Howlers is not null)
                {
                    foreach (var howler in Howlers)
                    {
                        Hunt(howler, out _, queryName);
                    }
                }

            }
            catch
            {
                throw new Exception();
            }
        }

        public void Hunt(IHowler dispatch, out Wolfpack result, string queryName = "Query")
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

        public void Hunt(IDynamicHowler dispatch, out DynamicWolfpack result, string queryName = "Query")
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

        public async void HuntAsync(string queryName = "Query")
        {
            Revit.Async.RevitTask.Initialize(_app);
            //Debug.Print(Howlers.Count.ToString());
            await RevitTask.RunAsync(() =>
            {
                try
                {
                    foreach (var howler in Howlers)
                    {
                        var result = howler.Howl();
                        //Debug.Print(result.ToString());
                        Queries.Add(queryName, result);
                        WriteDataToJson(result, "async", Desktop);
                    }


                    if (DynamicHowlers is not null)
                    {
                        foreach (var dynamicHowler in DynamicHowlers)
                        {
                            var result = dynamicHowler.Howl();
                            //Debug.Print(result.ToString());
                            DynamicQueries.Add(queryName, result);
                            WriteDataToJson(result, "dyn_async", Desktop);
                        }


                    }
                }
                catch
                {
                    throw new Exception();
                }
            });
        }

        public async void HuntAsync(IHowler dispatch, string queryName = "Query")
        {
            Revit.Async.RevitTask.Initialize(_app);
            await RevitTask.RunAsync(() =>
            {
                var results = dispatch.Howl();
                Queries.Add(queryName, results);
                //WriteDataToJson(results, "results.json")
                //WriteDataToJson(results, "test.json", Desktop);
            });
        }

        public async void HuntAsync(IDynamicHowler dispatch, string queryName = "Query")
        {
            Revit.Async.RevitTask.Initialize(_app);
            await RevitTask.RunAsync(() =>
            {
                var results = dispatch.Howl();
                DynamicQueries.Add(queryName, results);
            });
        }


        //public async void ExecuteHandledHunt(IDynamicHowler dispatch, string queryName = "Query")
        //{
        //    var query = await RevitTask.RunAsync(
        //        async () =>
        //        {
        //            //dispatch.Howl();
        //            var result = await RevitTask.RaiseGlobal<DirewolfDynHuntExternalEventHandler, IDynamicHowler, DynamicWolfpack>(dispatch);

        //            DynamicQueries.Add(queryName, result);
        //            WriteDataToJson("async.json");
        //        });



        //}

        private readonly string Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public static void WriteDataToJson(object data, string filename, string path)
        {
            string fileName = Path.Combine(path, $"{filename}.json");
            File.WriteAllText(fileName, JsonSerializer.Serialize(data));
        }

        public void WriteQueriesToJson()
        {
            string fileName = Path.Combine(Desktop, $"Queries.json");
            File.WriteAllText(fileName, JsonSerializer.Serialize(Queries));
        }
        public void WriteDynamicQueriesToJson()
        {
            string fileName = Path.Combine(Desktop, $"DynamicQueries.json");
            File.WriteAllText(fileName, JsonSerializer.Serialize(DynamicQueries));
        }

        public void ShowResultToGUI()
        {
            using StringWriter s = new();
            foreach (var result in Queries)
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
