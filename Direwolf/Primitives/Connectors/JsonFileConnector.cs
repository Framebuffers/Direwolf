using System.Diagnostics;
using System.Text.Json;
using Direwolf.Contracts;
using static System.String;

namespace Direwolf.Primitives.Connectors;

public readonly record struct JsonFileConnector(string FileName, string Path) : IConnector
{
    private string FullPath => System.IO.Path.Join(Path, FileName);

    public bool Connect()
    {
        if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);
        return true;
    }

    public bool Disconnect()
    {
        if (Directory.Exists(Path)) Directory.Delete(Path);
        return true;
    }

    public bool Write(List<IWolfpack>? wolfpack)
    {
        Debug.Print("Writing to JSON");
        if (wolfpack is null) return false;
        
        using StreamWriter writer = new(FullPath);
        foreach (var pack in wolfpack)
        {
            writer.Write(JsonSerializer.Serialize(pack.Data));    
        }
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