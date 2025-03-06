using Autodesk.Revit.UI;
using Direwolf.Contracts;
using Direwolf.Contracts.Dynamics;
using Direwolf.Definitions;
using Direwolf.Definitions.Dynamics;
using Direwolf.EventHandlers;
using IronPython.Compiler.Ast;
using Revit.Async;
using System.CodeDom;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Direwolf
{
    public sealed class Direwolf
    {
        public Direwolf() { }
        public Direwolf(IHowler howler)
        {
            Howlers.Enqueue(howler);
        }
        public Direwolf(IDynamicHowler howler)
        {
            DynamicHowlers.Enqueue(howler);
        }
        public Direwolf(IHowler howler, IHowl instructions)
        {
            howler.CreateWolf(new Wolf(), instructions);
            Howlers.Enqueue(howler);
        }

        public Direwolf(IDynamicHowler howler, IDynamicHowl instructions)
        {
            howler.CreateWolf(new DynamicWolf(), instructions);
            DynamicHowlers.Enqueue(howler);
        }

        public Direwolf(IHowler howler, IHowl instructions, IWolf wolf)
        {
            howler.CreateWolf(wolf, instructions);
            Howlers.Enqueue(howler);
        }

        public Direwolf(IDynamicHowler howler, IDynamicHowl instructions, IDynamicWolf wolf)
        {
            howler.CreateWolf(wolf, instructions);
            DynamicHowlers.Enqueue(howler);
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
            dynamic howlers = new DynamicCatch();
            howlers.Count = Howlers.Count;

            foreach (IHowler h in Howlers)
            {
                dynamic howler = new DynamicCatch();
                dynamic hw = new DynamicCatch();
                hw.Wolves = h.Wolfpack.Count;
                hw.CatchCount = h.Den.Count;
                howler.Name = h.GetType().Name;
                howler.Metadata = hw;
                howlers.Howler = howler;
            }

            dynamic dynHowlers = new DynamicCatch();
            dynHowlers.Count = DynamicHowlers.Count;

            foreach (IDynamicHowler h in DynamicHowlers)
            {
                dynamic howler = new DynamicCatch();
                dynamic hw = new DynamicCatch();
                hw.Wolves = h.Wolfpack.Count;
                hw.CatchCount = h.Den.Count;
                howler.Name = h.GetType().Name;
                howler.Metadata = hw;
                howlers.Howler = howler;
            }

            dynamic total = new DynamicCatch();
            total.TotalHowlers = howlers;
            total.TotalDynamicHowlers = dynHowlers;

            return JsonSerializer.Serialize(total);
        }

        [JsonExtensionData]
        public Dictionary<string, Wolfpack> Queries { get; set; } = [];

        [JsonExtensionData]
        public Dictionary<string, DynamicWolfpack> DynamicQueries { get; set; } = [];

        public string GetQueryInfo()
        {
            dynamic queries = new DynamicCatch();
            queries.Count = Queries.Count;

            dynamic queryInfo = new DynamicCatch();            
            foreach (var query in Queries)
            {
                dynamic queryData = new DynamicCatch();
                queryData.Name = query.Key;
                dynamic wolfpack = new DynamicCatch();
                wolfpack.DateTime = query.Value.Timestamp;
                wolfpack.GUID = query.Value.GUID;
                wolfpack.ResultCount = query.Value.ResultCount;
                queryData.Wolfpack = wolfpack;
                queryInfo.Query = queryData;
            }

            dynamic dynQueries = new DynamicCatch();
            queries.Count = DynamicQueries.Count;

            dynamic dynQueryInfo = new DynamicCatch();            
            foreach (var query in DynamicQueries)
            {
                dynamic queryData = new DynamicCatch();
                queryData.Name = query.Key;
                dynamic wolfpack = new DynamicCatch();
                wolfpack.DateTime = query.Value.Timestamp;
                wolfpack.GUID = query.Value.GUID;
                wolfpack.ResultCount = query.Value.ResultCount;
                queryData.Wolfpack = wolfpack;
                queryInfo.Query = queryData;
            }

            dynamic totals = new DynamicCatch();
            totals.Queries = queries;
            totals.DynamicQueries = dynQueries;
            totals.TotalQueries = Queries.Count + DynamicQueries.Count;

            return JsonSerializer.Serialize(totals);
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

                if (DynamicHowlers is not null)
                {
                    foreach (var dynamicHowler in DynamicHowlers)
                    {
                        Hunt(dynamicHowler, out _, queryName);
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
            await RevitTask.RunAsync(() =>
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

                    if (DynamicHowlers is not null)
                    {
                        foreach (var dynamicHowler in DynamicHowlers)
                        {
                            Hunt(dynamicHowler, out _, queryName);
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
            await RevitTask.RunAsync(() =>
            {
                Hunt(dispatch, out _, queryName);
            });
        }

        public async void HuntAsync(IDynamicHowler dispatch, string queryName = "Query")
        {
            await RevitTask.RunAsync(() =>
            {
                Hunt(dispatch, out _, queryName);
            });
        }


        //public async void ExecuteHandledHunt(IDynamicHowler dispatch, string queryName = "Query")
        //{
        //    //var query = await RevitTask.RunAsync(
        //    //    async () =>
        //    //    {
        //    //        //dispatch.Howl();
        //    //        var result = await RevitTask.RaiseGlobal<DirewolfDynHuntExternalEventHandler, IDynamicHowler, DynamicWolfpack>(dispatch);

        //    //        DynamicQueries.Add(queryName, result);
        //    //        WriteDataToJson("async.json");
        //    //    });



        //}

        private readonly string Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public static void WriteDataToJson(object data, string filename, string path)
        {
            string fileName = Path.Combine(path, $"{filename}.json");
            File.WriteAllText(fileName, JsonSerializer.Serialize(data));
        }

        public void WriteQueriesToJson()
        {
            string fileName = Path.Combine(Desktop, $"{Queries}.json");
            File.WriteAllText(fileName, JsonSerializer.Serialize(Queries));
        }
        public void WriteDynamicQueriesToJson()
        {
            string fileName = Path.Combine(Desktop, $"{DynamicQueries}.json");
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
