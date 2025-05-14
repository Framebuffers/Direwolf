namespace Direwolf.Dto.InternalDb.Enums;

/// <summary>
///     The bounds of the set that will be queried and returned as a result.
/// </summary>
public enum Realm
{
    DirewolfQuery,
    DirewolfSet,
    DirewolfInternal,
    Parameter,
    Instances,
    Element,
    ElementType,
    Group,
    View,
    Schedule,
    Link,
    Family,
    Category,
    Document,
    RevitDatabase,
    RevitUIApplication,
    File,
    LocalWorkshare,
    CloudWorkshare,
    DataWarehouse
}