using Direwolf.Sources.InternalDB;

namespace Direwolf.EventArgs;

public class DatabaseOperationEventArgs : System.EventArgs
{
    public Database Database {get; set;}
}