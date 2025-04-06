using System.Diagnostics;
using Direwolf.Contracts;

namespace Direwolf.Definitions.Connectors;

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

    public bool Create(Direwolf w)
    {
        Debug.Print("Create");
        Debug.Print(w.ToString());
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