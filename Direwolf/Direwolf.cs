using Autodesk.Revit.UI;
using Direwolf.Contracts;
using Direwolf.Definitions;
using Direwolf.EventHandlers;
using Revit.Async;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Direwolf
{
    public class Direwolf
    {
        /// <summary>
        /// This is a proof of concept, not a production-ready solution. Please **CHANGE THIS** if you plan to deploy.
        /// </summary>
        private static readonly DbConnectionString _default = new("direwolf", "wolf", "awoo", "direwolf");

        private readonly UIApplication? _app;
        private List<HowlId> PreviousHowls = [];

        public Direwolf(UIApplication app) 
        {
            _app = app;  
        }

        public Direwolf(IHowler howler, UIApplication app)
        {
            Howlers.Enqueue(howler);
            howler.HuntCompleted += OnHuntCompleted;
            _app = app;
        }
        public Direwolf(IHowler howler, IHowl instructions, UIApplication app)
        {
            howler.CreateWolf(new Wolf(), instructions);
            Howlers.Enqueue(howler);
            howler.HuntCompleted += OnHuntCompleted;
            _app = app;
        }

        public Direwolf(IHowler howler, IHowl instructions, IWolf wolf, UIApplication app)
        {
            howler.CreateWolf(wolf, instructions);
            Howlers.Enqueue(howler);
            howler.HuntCompleted += OnHuntCompleted;
            _app = app;
        }


        public void QueueHowler(IHowler howler)
        {
            ArgumentNullException.ThrowIfNull(howler);
            Howlers.Enqueue(howler);
            howler.HuntCompleted += OnHuntCompleted;
        }

        private Queue<IHowler> Howlers { get; set; } = [];

        public string GetQueueInfo()
        {
            try
            {
                var howlers = new Dictionary<string, object>
                {
                    ["count"] = Howlers.Count.ToString() ?? ""
                };

                foreach (IHowler h in Howlers)
                {
                    var howler = new Dictionary<string, object>();
                    var hw = new Dictionary<string, object>
                    {
                        ["wolves"] = h.Wolfpack.Count,
                        ["catchCount"] = h.Den.Count
                    };

                    howler["name"] = h.GetType().Name;
                    howler["metadata"] = hw;
                    howlers["howler"] = howler;
                }

                var total = new Dictionary<string, object>
                {
                    ["totalHowlers"] = howlers
                };
                return JsonSerializer.Serialize(new Prey(total));
            }
            catch
            {
                return "";
            }
        }

        [JsonExtensionData]
        private WolfpackDB Queries { get; set; } = new(_default);
        public async void Push(Wolfpack w) => await Queries.Add(w);
        public async void SendAllToDB() => await Queries.Flush();

        //public string GetQueryInfo()
        //{
        //    try
        //    {
        //        var queries = new Dictionary<string, object>
        //        {
        //            ["count"] = Queries.Count
        //        };

        //        var queryInfo = new Dictionary<string, object>();
        //        foreach (var query in Queries)
        //        {
        //            var queryData = new Dictionary<string, object>
        //            {
        //                ["name"] = query.Key,
        //                ["wolfpack"] = new Dictionary<string, object>
        //                {
        //                    ["dateTime"] = query.Value.CreatedAt,
        //                    ["id"] = query.Value.GUID,
        //                    ["resultCount"] = query.Value.ResultCount
        //                }
        //            };

        //            queryInfo["query"] = queryData;
        //        }


        //        var totals = new Dictionary<string, object>
        //        {
        //            ["queries"] = queries,
        //            ["totalQueries"] = Queries.Count
        //        };
        //        return JsonSerializer.Serialize(new Prey(totals));
        //    }
        //    catch
        //    {
        //        return "";
        //    }

        //}

        public void Hunt(string queryName = "query")
        {
            try
            {
                if (Howlers is not null)
                {
                    foreach (var howler in Howlers)
                    {
                        Hunt(howler, out _, queryName);
                        var h = new HowlId()
                        {
                            HowlIdentifier = new Guid(),
                            Name = howler.GetType().Name
                        };
                        PreviousHowls.Add(h);
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
                Queries.Push(result);
                var h = new HowlId()
                {
                    HowlIdentifier = new Guid(),
                    Name = dispatch.GetType().Name
                };
                PreviousHowls.Add(h);
            }
            catch
            {
                throw new Exception();
            }
        }

        public async void HuntAsync(string? path = null, string queryName = "query")
        {
            Revit.Async.RevitTask.Initialize(_app);
            await RevitTask.RunAsync(() =>
            {
                try
                {
                    foreach (var howler in Howlers)
                    {
                        var result = howler.Howl();
                        Queries.Push(result);
                        var h = new HowlId()
                        {
                            HowlIdentifier = new Guid(),
                            Name = howler.GetType().Name
                        };
                        PreviousHowls.Add(h);
                    }
                }
                catch
                {
                    throw new Exception();
                }
            });
        }

        public async void HuntAsync(IHowler howler, string queryName = "query")
        {
            Revit.Async.RevitTask.Initialize(_app);
            await RevitTask.RunAsync(() =>
            {
                var results = howler.Howl();
                Queries.Push(results);
                var h = new HowlId()
                {
                    HowlIdentifier = new Guid(),
                    Name = howler.GetType().Name
                };
                PreviousHowls.Add(h);
            });
        }

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

        public string GetQueriesAsJson()
        {
            return JsonSerializer.Serialize(Queries);
        }

        private void OnHuntCompleted(object? sender, HuntCompletedEventArgs e)
        {
            Debug.Print("HuntSuccessful?: " + e.IsSuccessful.ToString());
        }
    }

}
