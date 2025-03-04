using Direwolf.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Contracts
{
    public interface IHowler
    {
        public Stack<Catch> Den { get; set; }
        public List<IWolf> Wolfpack { get; set; }
        public void CreateWolf(IWolf runner, IHowl instruction);
        public Wolfpack Howl();
    }
}
