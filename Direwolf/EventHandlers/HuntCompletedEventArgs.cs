using Direwolf.Definitions;

namespace Direwolf.EventHandlers
{
    public class HuntCompletedEventArgs : EventArgs
    {
        public bool IsSuccessful { get; set; }
        public WolfpackTarget Where { get; set; }
    }
 }
