using Direwolf.Definitions;

namespace Direwolf.Contracts;

/// <summary>
///     Dispatch source from where <see cref="IWolf" /> runners are sent, <see cref="IHowl" /> instructions are set for
///     each Direwolf, and the method to initiate the query is held. It holds the results inside <see cref="IHowler.Den" />
///     as
///     <see cref="Prey" />, to then be serialized as <see cref="WolfQueue" />.
/// </summary>
public interface IHowler
{
    public Wolfpack GenerateResults(string resultsName);
    public Task Awoo();
    public Prey Pop();
    public void Push(Prey p);
}