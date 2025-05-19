using Direwolf.Dto.Driver;
using Direwolf.Dto.InternalDb.Enums;

namespace Direwolf.Contracts;

public interface IWolfpack
{
    public DocumentType          DocumentType {get; set;}
    public Realm                 Realm        {get; set;}
    public List<DriverCommonDto> Drivers      {get; set;}
}