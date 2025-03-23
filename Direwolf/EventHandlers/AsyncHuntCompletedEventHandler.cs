using Direwolf.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.EventHandlers
{
    public class AsyncHuntCompletedEventHandler : EventArgs
    {
        public bool IsSuccessful { get; set; }
        public WolfpackTarget Destination { get; set; }
    }
}
