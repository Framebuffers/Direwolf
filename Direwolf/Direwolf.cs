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
    public partial class Direwolf
    {
        public event EventHandler? DatabaseConnectionEventHandler;
        public event EventHandler<AsyncHuntCompletedEventHandler>? AsyncHuntCompletedEventHandler;
        private Queue<IHowler> Howlers { get; set; } = [];
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

        private void Direwolf_AsyncHuntCompletedEventHandler(object? sender, AsyncHuntCompletedEventHandler e)
        {
            switch (e.Destination)
            {
                case WolfpackTarget.OnScreen:
                    break;
                case WolfpackTarget.Excel:
                    break;
                case WolfpackTarget.DB:
                    break;
                case WolfpackTarget.INVALID:
                default:
                    break;

            }
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


        public async void SendAllToDB()
        {
            try { await Queries.Send(); } catch (Exception e) { Debug.Print(e.Message); }
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
