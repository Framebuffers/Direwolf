using System.Collections.ObjectModel;
using Direwolf.Definitions.Internal;
using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.Parsers;

namespace Direwolf.Definitions;

public readonly record struct Wolfpack(
    Cuid Id,
    Method? Method,
    string? Name,
    string? Description,
    ReadOnlyDictionary<string, string>? Properties,
    IReadOnlyList<Howl>? Payload)
{
    public static Wolfpack CreateInstance(Cuid? id,
        Method method,
        string? name,
        string? description,
        ReadOnlyDictionary<string, string>? properties,
        IReadOnlyList<Howl> payload)
    {
        var wp = new Wolfpack(id ?? Cuid.Create(), method, name, description, properties, payload);  
        WolfpackCreated?.Invoke(wp, EventArgs.Empty);
        return wp;
    }

    public static event EventHandler? WolfpackCreated;
    
};