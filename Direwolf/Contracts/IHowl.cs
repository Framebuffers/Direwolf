using Direwolf.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Contracts
{
    public interface IHowl
    {
        public IWolf? Callback { get; set; } // making it nullable sounds like a bad idea
        public bool Execute();
        public void PushCatchesToWolf(Catch c);
    }

}
