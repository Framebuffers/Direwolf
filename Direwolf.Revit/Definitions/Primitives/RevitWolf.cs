using System.Diagnostics;
using System.Text.Json.Serialization;
using Autodesk.Revit.DB;
using Direwolf.Contracts;
using Direwolf.Revit.Contracts;
using IConnector = Direwolf.Contracts.IConnector;

namespace Direwolf.Revit.Definitions.Primitives;

public record RevitWolf : IRevitWolf
{
    private readonly RevitHowl? _revitHowl;

    /// <summary>
    ///     Inside a wolf there is two things: who summoned you, and what you need to do.
    ///     When the Direwolf calls Howls(), the Direwolf attaches itself to the howl
    ///     and executes the instruction inside the Howls.
    /// </summary>
    public RevitWolf(IHowler callback, RevitHowl instruction, List<IConnector> destinations, Document doc)
    {
        
        _revitHowl = instruction;
        _revitHowl = instruction;
        _revitHowl.SetRevitDocument(doc);
        Summoner = callback;
        Instruction = instruction;
        Destinations = destinations;
    }

    public RevitWolf(IHowler callback, RevitHowl instruction, IConnector destination, Document doc)
    {
        _revitHowl = instruction;
        _revitHowl = instruction;
        _revitHowl.SetRevitDocument(doc);
        Summoner = callback;
        Instruction = instruction;
        Destinations.Add(destination);
    }

    [JsonIgnore] public IHowler Summoner { get; init; }
    [JsonIgnore] public IHowl Instruction { get; init; }

    IWolfpack? IWolf.Result
    {
        get => Result;
        set
        {
            if (value is RevitWolfpack rw)
                Result = rw;
        }
    }

    [JsonIgnore] public RevitWolfpack Result { get; set; }

    public void Hunt()
    {
        Debug.Print("Hunting started");
        if (Instruction is null || Summoner is null || Destinations is null)
            throw new NullReferenceException();
        Debug.Print("Inside Hunt loop");
        Result = _revitHowl?.ExecuteHunt() ?? throw new NullReferenceException();
    }

    [JsonIgnore] public List<IConnector> Destinations { get; init; } = [];

    public void Deconstruct(out IHowler callback, out IHowl? instruction)
    {
        callback = Summoner;
        instruction = Instruction;
    }

    // don't serialize this record: it's just a vessel for the Task being run
    public override string ToString()
    {
        return GetType().Name;
    }
}