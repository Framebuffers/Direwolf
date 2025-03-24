using Autodesk.Revit.UI;
using Direwolf.Contracts;
using Direwolf.Definitions;
using Direwolf.EventHandlers;
using System.Diagnostics;

namespace Direwolf
{
    public partial class Direwolf
    {
        public event EventHandler? DatabaseConnectionEventHandler;
        public event EventHandler<HuntCompletedEventArgs>? HuntCompletedEventHandler;
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
            howler.CreateWolf(new Wolf(), instructions, howler.FinalTarget);
            Howlers.Enqueue(howler);
            howler.HuntCompleted += OnHuntCompleted;
            Queries.DatabaseConnectedEventHandler += Queries_DatabaseConnectedEventHandler;
            _app = app;
        }


        public Direwolf(IHowler howler, IHowl instructions, IWolf wolf, UIApplication app)
        {
            howler.CreateWolf(wolf, instructions, howler.FinalTarget);
            Howlers.Enqueue(howler);
            howler.HuntCompleted += OnHuntCompleted;
            Queries.DatabaseConnectedEventHandler += Queries_DatabaseConnectedEventHandler;
            _app = app;
        }
    }
}
