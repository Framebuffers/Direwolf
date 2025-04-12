using System.Diagnostics;
using Direwolf.Contracts;

namespace Direwolf.Primitives.Connectors;

public readonly record struct DisplayOnScreenConnector : IConnector
{
    public bool Connect()
    {
        Debug.Print("Connect");
        return true;
    }

    public bool Disconnect()
    {
        Debug.Print("Disconnect");
        return true;
    }

    public bool Create(List<IWolfpack>? wolfpack)
    {
        Debug.Print("test 1 connector");
        foreach (var pack in wolfpack) Debug.Print(pack.Data.ToString());
        Debug.Print($"{DateTime.Now.ToUniversalTime()}");
        return true;
    }

    public Wolfpack[] Read(Direwolf w)
    {
        // return w.ToArray();
        throw new NotImplementedException();
    }

    public bool Update(Direwolf w)
    {
        throw new NotImplementedException();
    }

    public bool Destroy(Direwolf w)
    {
        throw new NotImplementedException();
    }
}