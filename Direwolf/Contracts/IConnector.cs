using Direwolf.Definitions;

namespace Direwolf.Contracts;

public interface IConnector
{
    public bool Connect();
    public bool Disconnect();
    public bool Create(Direwolf w);
    public Wolfpack[]? Read(Direwolf w);
    public bool Update(Direwolf w);
    public bool Destroy(Direwolf w);
}