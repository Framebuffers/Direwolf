using Direwolf.Definitions.Internal.Enums;

namespace Direwolf.EventArgs;

public class DatabaseChangedEventArgs : System.EventArgs
{
    public CrudOperation Operation {get; set;}
}