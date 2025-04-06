using System.Text.Json.Serialization;
using Autodesk.Revit.DB;
using Direwolf.Contracts;
using Direwolf.Definitions;
using Direwolf.Revit.Contracts;

namespace Direwolf.Revit.Howls;

public abstract record RevitHowl : IRevitHowl
{
    private Document? _rvtDoc;
    public string Name { get; set; }
    [JsonIgnore] public IWolf? Wolf { get; set; }
    public void SendWolfpackBack(IWolfpack c) => Wolf?.Summoner.Push(c);

    public abstract bool Hunt();
    
    public Document GetRevitDocument()
    {
        ArgumentNullException.ThrowIfNull(_rvtDoc);
        return _rvtDoc;
    }

    public void SetRevitDocument(Document value)
    {
        ArgumentNullException.ThrowIfNull(value);
        _rvtDoc = value;
    }
    
    public override string ToString() => Name;
}