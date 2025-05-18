using Direwolf.Dto.Driver;
using Direwolf.Dto.InternalDb.Enums;
using Direwolf.Dto.Parser;

namespace Direwolf.Contracts;

public interface IWolfpack
{
    public DocumentType DocumentType { get; set; }
    public Realm Realm { get; set; }
    public List<DriverCommonDto> Drivers { get; set; }
}