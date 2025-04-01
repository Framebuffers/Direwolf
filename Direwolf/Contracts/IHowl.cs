using Direwolf.Definitions;

namespace Direwolf.Contracts
{
/// <summary>
/// Instruction for a <see cref="IWolf"/> to perform.
/// </summary>
    public interface IHowl
    {
        public IWolf? Callback { get; set; } // making it nullable sounds like a bad idea
        public bool Execute();
        public void SendCatchToCallback(Prey c);
    }

}
