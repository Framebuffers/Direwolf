using Direwolf.Definitions;
using Direwolf.Lonewolfs;

namespace Direwolf;

public interface ILonewolfConnector
{
    public bool Connect();
    public bool Disconnect();
    public bool Create(Lonewolf w);
    public Wolfpack[]? Read(); 
    public bool Update(Lonewolf w);
    public bool Destroy(Lonewolf w);
}

public interface IJsonFileLonewolf : ILonewolfConnector
{
    public string FilePath { get; }
}

public interface IBsonFileLonewolf : ILonewolfConnector
{
    public string FilePath { get; }
}

public interface IExcelFileLonewolf : ILonewolfConnector
{
    public string FilePath { get; }
}

public interface ICsvFileLonewolf : ILonewolfConnector
{
    public string FilePath { get; }
}

