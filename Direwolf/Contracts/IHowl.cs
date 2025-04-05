using Direwolf.Definitions;

namespace Direwolf.Contracts;

/// <summary>
///     Instruction for a <see cref="IWolf" /> to perform.
/// </summary>
public interface IHowl
{
    public string Name { get; set; }
    public Wolf? Callback { get; set; }
    public bool Execute();
    public void SendCatchToCallback(Prey c);
}