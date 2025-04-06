namespace Direwolf.Contracts;

public interface IWolf
{
    public IHowler Summoner { get; init; }
    public IHowl Instruction { get; init; }
    public bool Run();
    
}