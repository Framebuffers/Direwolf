using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.EventHandlers
{
    public class HuntCompletedEventArgs : EventArgs
    {
        public bool IsSuccessful { get; set; }
    }
}
