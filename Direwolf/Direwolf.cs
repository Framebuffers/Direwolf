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
        public event EventHandler? DatabaseConnectionEventHandler;
        public event EventHandler? AsyncHuntCompletedEventHandler;


        /// <summary>
        /// This is a proof of concept, not a production-ready solution. Please **CHANGE THIS** if you plan to deploy.
        /// </summary>
        private static readonly DbConnectionString _default = new("localhost", 5432, "wolf", "awoo", "direwolf");

        private readonly UIApplication? _app;
        private List<HowlId> PreviousHowls = [];

        public Direwolf(UIApplication app)
        {
            Queries.DatabaseConnectedEventHandler += Queries_DatabaseConnectedEventHandler;
            _app = app;
        }

        public Direwolf(IHowler howler, UIApplication app)
        {
            Howlers.Enqueue(howler);
            howler.HuntCompleted += OnHuntCompleted;
            Queries.DatabaseConnectedEventHandler += Queries_DatabaseConnectedEventHandler;
            _app = app;
        }

        private void Queries_DatabaseConnectedEventHandler(object? sender, EventArgs e)
        {
            Debug.Print("Database connected!");
        }

        public Direwolf(IHowler howler, IHowl instructions, UIApplication app)
        {
            howler.CreateWolf(new Wolf(), instructions);
            Howlers.Enqueue(howler);
            howler.HuntCompleted += OnHuntCompleted;
            Queries.DatabaseConnectedEventHandler += Queries_DatabaseConnectedEventHandler;
            _app = app;
        }

        private void Direwolf_AsyncHuntCompletedEventHandler(object? sender, EventArgs e)
        {
            SendAllToDB();
        }

        public Direwolf(IHowler howler, IHowl instructions, IWolf wolf, UIApplication app)
        {
            howler.CreateWolf(wolf, instructions);
            Howlers.Enqueue(howler);
            howler.HuntCompleted += OnHuntCompleted;
            Queries.DatabaseConnectedEventHandler += Queries_DatabaseConnectedEventHandler;

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
        public async void SendAllToDB()
        {
            try
            {
                foreach (var q in Queries)
                {
                    string fileName = Path.Combine(Desktop, $"Queries.json");
                    File.WriteAllText(fileName, q.Results.ToString());
                }
                Debug.Print(Queries.Count.ToString());

                await Queries.Send();
            }
            catch (Exception e) { Debug.Print(e.Message); }
        }

        public void Hunt(string testName)
        {
            try
            {
                foreach (var howler in Howlers)
                {
                    Hunt(howler, out _, testName);
                    var h = new HowlId()
                    {
                        HowlIdentifier = new Guid(),
                        Name = howler.GetType().Name
                    };
                    PreviousHowls.Add(h);
                    Debug.Print("Added to queue");
                }
            }
            catch
            {
                throw new Exception();
            }
        }

        public void Hunt(IHowler dispatch, out Wolfpack result, string testName)
        {
            try
            {
                result = dispatch.Howl(testName);
                var h = new HowlId()
                {
                    HowlIdentifier = new Guid(),
                    Name = dispatch.GetType().Name
                };
                PreviousHowls.Add(h);
                Queries.Push(result);
            }
            catch
            {
                throw new Exception();
            }
        }

        public async void HuntAsync(string queryName = "query")
        {
            AsyncHuntCompletedEventHandler += Direwolf_AsyncHuntCompletedEventHandler1;
            Revit.Async.RevitTask.Initialize(_app);
            foreach (var howler in Howlers)
            {
                await HuntTask(howler, queryName);
            }
        }

        private void Direwolf_AsyncHuntCompletedEventHandler1(object? sender, EventArgs e)
        {
            SendAllToDB();
        }

        public async void HuntAsync(IHowler howler, string queryName = "query")
        {
            Revit.Async.RevitTask.Initialize(_app);
            await HuntTask(howler, queryName);
        }

        private async Task HuntTask(IHowler howler, string queryName = "query")
        {
            try
            {
                Revit.Async.RevitTask.Initialize(_app);
                await RevitTask.RunAsync(() =>
                {
                    var results = howler.Howl(queryName);
                    Queries.Push(results);
                    var h = new HowlId()
                    {
                        HowlIdentifier = new Guid(),
                        Name = howler.GetType().Name
                    };
                    PreviousHowls.Add(h);
                });
                AsyncHuntCompletedEventHandler?.Invoke(this, new EventArgs());
            }
            catch (Exception e) { Debug.WriteLine(e.Message); }

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
