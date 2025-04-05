namespace Direwolf.EventHandlers;

public class HuntCompletedEventArgs : EventArgs
{
    public HuntCompletedEventArgs()
    {
    }

    public HuntCompletedEventArgs(bool isSuccessful)
    {
        IsSuccessful = isSuccessful;
    }

    public bool IsSuccessful { get; set; }
}