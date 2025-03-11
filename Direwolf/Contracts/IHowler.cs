using Direwolf.Definitions;
using Direwolf.EventHandlers;

namespace Direwolf.Contracts
{
    public interface IHowler
    {
        public Stack<Prey> Den { get; set; }
        public Queue<IWolf> Wolfpack { get; set; }
        public void CreateWolf(IWolf runner, IHowl instruction);
        public Wolfpack Howl();
        public event EventHandler<HuntCompletedEventArgs> HuntCompleted;
    }
}
