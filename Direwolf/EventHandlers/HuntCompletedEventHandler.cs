namespace Direwolf.EventHandlers;

public class HuntCompletedEventArgs : EventArgs
{
    public bool IsSuccessful { get; set; }
}