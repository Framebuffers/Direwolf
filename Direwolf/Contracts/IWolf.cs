using Direwolf.Definitions;

namespace Direwolf.Contracts;

/// <summary>
///     Runner that performs the task held in <see cref="IWolf.Instruction" />, to then send results back to
///     <see cref="IHowler" />.
/// </summary>
public interface IWolf
{
    public IHowler? Callback { get; set; }
    public IHowl? Instruction { get; set; }
    public Stack<Prey> Catches { get; set; }
    public bool Run();
}