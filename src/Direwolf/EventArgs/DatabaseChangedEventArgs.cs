using Direwolf.Dto.InternalDb.Enums;

namespace Direwolf.EventArgs;

public class DatabaseChangedEventArgs : System.EventArgs
{
    public CrudOperation Operation {get; set;}
}