using Direwolf.Primitives;

namespace Direwolf.Contracts;

public interface IConnector
{
    public bool Connect();
    public bool Disconnect();
    public bool Write(List<IWolfpack>? wolfpacks);
    public Wolfpack[]? Read(Direwolf w);
    public bool Update(Direwolf w);
    public bool Destroy(Direwolf w);
}