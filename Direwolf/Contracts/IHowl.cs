using Direwolf.Definitions;

namespace Direwolf.Contracts
{
    public interface IHowl
    {
        public IWolf? Callback { get; set; } // making it nullable sounds like a bad idea
        public bool Execute();
        public void SendCatchToCallback(Prey c);
    }

}
