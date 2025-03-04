using Direwolf.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Contracts
{
    public interface IWolf
    {
        public IHowler? Callback { get; set; }
        public IHowl? Instruction { get; set; }
        public Stack<Catch> Catches { get; set; }
        public bool Run();
    }
}
