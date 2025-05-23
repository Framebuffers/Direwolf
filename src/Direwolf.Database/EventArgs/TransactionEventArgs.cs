using Autodesk.Revit.DB;

using Direwolf.Definitions.Internal.Enums;

using Transaction = Direwolf.Definitions.Internal.Transaction;

namespace Direwolf.Database.EventArgs;

public class TransactionEventArgs : System.EventArgs
{
    public CrudOperation Operation { get; set; }
    public Transaction Transaction { get; set; }
}