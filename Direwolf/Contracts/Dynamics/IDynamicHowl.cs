using Direwolf.Definitions.Dynamics;

namespace Direwolf.Contracts.Dynamics
{
    public interface IDynamicHowl
    {
        public IDynamicWolf? Callback { get; set; } // making it nullable sounds like a bad idea
        public bool Execute();
        public void SendCatchToCallback(DynamicCatch c);
    }
}
