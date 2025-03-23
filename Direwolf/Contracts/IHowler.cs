using Direwolf.Definitions;
using Direwolf.EventHandlers;

namespace Direwolf.Contracts
{
    public interface IHowler
    {
        public Stack<Prey> Den { get; set; }
        public Queue<IWolf> Wolfpack { get; set; }
        public void CreateWolf(IWolf runner, IHowl instruction);
        public Wolfpack Howl(string testName, DbConnectionString db, WolfpackTarget where);
        public Wolfpack Howl(string testName, WolfpackTarget where);
        public event EventHandler<HuntCompletedEventArgs> HuntCompleted;

    }
}
