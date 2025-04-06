using Direwolf.Definitions;

namespace Direwolf.Contracts;

/// <summary>
///     Instruction for a <see cref="IWolf"/> to perform.
/// </summary>
public interface IHowl
{
    public string? Name { get; set; }
    public IWolf Wolf { get; set; }
    public bool Hunt();
    public void SendWolfpackBack(IWolfpack c);
}