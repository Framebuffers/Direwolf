using Direwolf.Definitions.Internal.Enums;

namespace Direwolf.EventArgs;

/// <summary>
///     Defines which kind of operation has been performed on the DB: CreateRevitId, Read, Update or Delete.
/// </summary>
public class DatabaseChangedEventArgs : System.EventArgs
{
    /// <summary>
    ///     Specifies one of four operations: CreateRevitId, Read, Update or Delete.
    /// </summary>
    public CrudOperation Operation { get; init; }
}