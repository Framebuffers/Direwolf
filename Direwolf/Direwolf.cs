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
    public partial class Direwolf : IDirewolf
    {
        public event EventHandler? AsyncHuntCompletedEventHandler;

        private readonly UIApplication? _app;
        private List<HowlId> PreviousHowls = [];
        private Queue<IHowler> Howlers { get; set; } = [];

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

        public Direwolf(IHowler howler, IHowl instructions, UIApplication app)
        {
            howler.CreateWolf(new Wolf(), instructions);
            Howlers.Enqueue(howler);
            howler.HuntCompleted += OnHuntCompleted;
            Queries.DatabaseConnectedEventHandler += Queries_DatabaseConnectedEventHandler;
            _app = app;
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

        private void OnHuntCompleted(object? sender, HuntCompletedEventArgs e)
        {
            Debug.Print("HuntSuccessful?: " + e.IsSuccessful.ToString());
        }

    }

}
