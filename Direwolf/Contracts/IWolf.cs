namespace Direwolf.Contracts;

public interface IWolf
{
    public IHowler Summoner { get; init; }
    public IHowl Instruction { get; init; }
    public IWolfpack? Result { get; set; }
    public List<IConnector> Destinations { get; init; }
    public void Hunt();
}