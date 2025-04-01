using Direwolf.Definitions;
using Direwolf.EventHandlers;

namespace Direwolf.Contracts
{
    /// <summary>
    /// Dispatch source from where <see cref="IWolf"/> runners are sent, <see cref="IHowl"/> instructions are set for each Wolf, and the method to initiate the query is held. It holds the results inside <see cref="IHowler.Den"/> as <see cref="Prey"/>, to then be serialized as <see cref="WolfQueue"/>.
    /// </summary>
    public interface IHowler
    {
        public Stack<Prey> Den { get; set; }
        public Queue<IWolf> WolfQueue { get; set; }
        public void CreateWolf(IWolf runner, IHowl instruction);
        public Wolfpack Howl(string testName);
        public event EventHandler<HuntCompletedEventArgs> HuntCompleted;
    }
}
