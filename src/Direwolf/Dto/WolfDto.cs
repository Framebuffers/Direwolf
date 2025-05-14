using Autodesk.Revit.DB;
using Direwolf.Dto.Driver;
using Direwolf.Dto.InternalDb;
using Direwolf.Dto.InternalDb.Enums;
using Direwolf.Dto.Parser;

namespace Direwolf.Dto;

public record WolfDto(
    double Id,
    Guid UniqueId,
    Realm Realm,
    BuiltInCategory Category,
    DriverCommonDto DriverCommonDto,
    DataType DataType,
    DateTime CreatedDate
)
{
    public virtual object Data { get; set; } = null!;
}