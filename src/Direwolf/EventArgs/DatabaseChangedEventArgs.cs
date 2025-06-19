using Direwolf.Definitions.Enums;

namespace Direwolf.EventArgs;

/// <summary>
///     Defines which kind of operation has been performed on the DB: CreateRevitId, ReadRevitElements, Update or DeleteRevitElement.
/// </summary>
public class DatabaseChangedEventArgs : System.EventArgs
{
    /// <summary>
    ///     Specifies one of four operations: CreateRevitId, ReadRevitElements, Update or DeleteRevitElement.
    /// </summary>
    public MessageType Operation { get; init; }
}