using System.Collections.ObjectModel;
using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.Parsers;

namespace Direwolf.Definitions;

public readonly record struct Wolfpack(
    Cuid Id,
    Method? Method,
    string? Name,
    ReadOnlyDictionary<string, object>? Parameters,
    IReadOnlyList<Howl>? Data)
{
    public static Wolfpack CreateInstance(Cuid? id,
        Method method,
        string? name,
        ReadOnlyDictionary<string, object>? properties,
        IReadOnlyList<Howl> payload)
    {
        var wp = new Wolfpack(id ?? Cuid.Create(), method, name, properties, payload);
        WolfpackCreated?.Invoke(wp, EventArgs.Empty);
        return wp;
    }

    public static event EventHandler? WolfpackCreated;
}