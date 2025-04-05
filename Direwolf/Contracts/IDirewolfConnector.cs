using System.Runtime.CompilerServices;
using Direwolf.Definitions;

namespace Direwolf;

public interface IDirewolfConnector
{
    public bool Connect();
    public bool Disconnect();
    public bool Create(Direwolf w);
    public Prey[]? Read(Direwolf w);
    public bool Update(Direwolf w);
    public bool Destroy(Direwolf w);
}

public interface IJsonFileDirewolf : IDirewolfConnector
{
    public string FilePath { get; }
}

public interface IBsonFileDirewolf : IDirewolfConnector
{
    public string FilePath { get; }
}

public interface IExcelFileDirewolf : IDirewolfConnector
{
    public string FilePath { get; }
}

public interface ICsvFileDirewolf : IDirewolfConnector
{
    public string FilePath { get; }
}