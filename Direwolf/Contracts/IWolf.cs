using Direwolf.Definitions;

namespace Direwolf.Contracts
{
    public interface IWolf
    {
        public IHowler? Callback { get; set; }
        public IHowl? Instruction { get; set; }
        public Stack<Prey> Catches { get; set; }
        public bool Run();
    }
}
