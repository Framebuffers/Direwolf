using Direwolf.Definitions.Dynamics;

namespace Direwolf.Contracts.Dynamics
{
    public interface IDynamicWolf
    {
        public IDynamicHowler? Callback { get; set; }
        public IDynamicHowl? Instruction { get; set; }
        public Stack<DynamicCatch> Catches { get; set; }
        public bool Run();
    }
}
