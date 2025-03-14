using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Direwolf.Contracts;
using Direwolf.Definitions;
using Direwolf.EventHandlers;
using Revit.Async;
using System.Diagnostics;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Direwolf
{
    public class Direwolf
    {
        private readonly UIApplication? _app;
        private List<HowlId> PreviousHowls = [];

        public Direwolf(UIApplication app) { _app = app; }
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
                    ["Count"] = Howlers.Count.ToString() ?? ""
                };

                foreach (IHowler h in Howlers)
                {
                    var howler = new Dictionary<string, object>();
                    var hw = new Dictionary<string, object>
                    {
                        ["Wolves"] = h.Wolfpack.Count,
                        ["CatchCount"] = h.Den.Count
                    };

                    howler["Name"] = h.GetType().Name;
                    howler["Metadata"] = hw;
                    howlers["Howler"] = howler;
                }

                var total = new Dictionary<string, object>
                {
                    ["TotalHowlers"] = howlers
                };
                return JsonSerializer.Serialize(new Prey(total));
            }
            catch
            {
                return "";
            }
        }

        [JsonExtensionData]
        public Dictionary<string, Wolfpack> Queries { get; set; } = [];

        public string GetQueryInfo()
        {
            try
            {
                var queries = new Dictionary<string, object>
                {
                    ["Count"] = Queries.Count
                };

                var queryInfo = new Dictionary<string, object>();
                foreach (var query in Queries)
                {
                    var queryData = new Dictionary<string, object>
                    {
                        ["Name"] = query.Key,
                        ["Wolfpack"] = new Dictionary<string, object>
                        {
                            ["DateTime"] = query.Value.CreatedAt,
                            ["GUID"] = query.Value.GUID,
                            ["ResultCount"] = query.Value.ResultCount
                        }
                    };

                    queryInfo["Query"] = queryData;
                }


                var totals = new Dictionary<string, object>
                {
                    ["Queries"] = queries,
                    ["TotalQueries"] = Queries.Count
                };
                return JsonSerializer.Serialize(new Prey(totals));
            }
            catch
            {
                return "";
            }

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
                Queries.Add(queryName, result);
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
        public async void HuntAsync(string? path = null, string queryName = "Query")
        {
            Revit.Async.RevitTask.Initialize(_app);
            await RevitTask.RunAsync(() =>
            {
                try
                {
                    foreach (var howler in Howlers)
                    {
                        var result = howler.Howl();
                        Queries.Add(queryName, result);
                        var h = new HowlId()
                        {
                            HowlIdentifier = new Guid(),
                            Name = howler.GetType().Name
                        };
                        PreviousHowls.Add(h);
                        WriteDataToJson(result, "async", path ?? Desktop);
                    }
                }
                catch
                {
                    throw new Exception();
                }
            });
        }

        public async void HuntAsync(IHowler howler, string queryName = "Query")
        {
            Revit.Async.RevitTask.Initialize(_app);
            await RevitTask.RunAsync(() =>
            {
                var results = howler.Howl();
                Queries.Add(queryName, results);
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

        private void OnHuntCompleted(object? sender, HuntCompletedEventArgs e)
        {
            Debug.Print("HuntSuccessful?: " + e.IsSuccessful.ToString());
        }
    }

}
