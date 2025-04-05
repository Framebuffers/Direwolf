using System.Diagnostics;
using System.Text.Json;
using Direwolf.Definitions;
using MongoDB.Bson;

namespace Direwolf;

public readonly record struct DisplayOnScreenConnector : IDirewolfConnector
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

    public Prey[]? Read(Direwolf w)
    {
        return w.ToArray();
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