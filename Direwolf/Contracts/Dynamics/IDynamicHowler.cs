using Direwolf.Definitions;
using Direwolf.Definitions.Dynamics;

namespace Direwolf.Contracts.Dynamics
{
    public interface IDynamicHowler
    {
        public Stack<DynamicCatch> Den { get; set; }
        public Queue<IDynamicWolf> Wolfpack { get; set; }
        public void CreateWolf(IDynamicWolf runner, IDynamicHowl instruction);
        public DynamicWolfpack Howl();
    }
}
