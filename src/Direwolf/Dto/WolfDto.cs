using Autodesk.Revit.DB;

using Direwolf.Contracts;
using Direwolf.Dto.InternalDb.Enums;
using Direwolf.Dto.Parser;

using Transaction = Direwolf.Dto.InternalDb.Transaction;

#pragma warning disable VISLIB0001
namespace Direwolf.Dto;

/// <summary>
///     The base Data Operation Object type inside Direwolf. Implements <see cref="IWolf" />. Holds any operation
///     generated from an external source.
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="OperationType"></param>
/// <param name="UniqueId"></param>
/// <param name="Realm"></param>
/// <param name="Category"></param>
/// <param name="Driver"></param>
/// <param name="DataType"></param>
/// <param name="CreatedDate"></param>
public record WolfDto(Cuid UniqueId, string Name, Realm Realm, BuiltInCategory Category) : IWolf
{
    public virtual Transaction Data {get; set;}
}