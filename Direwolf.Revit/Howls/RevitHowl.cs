using System.Text.Json.Serialization;
using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Revit.Contracts;

namespace Direwolf.Revit.Howls;

public abstract record RevitHowl : IRevitHowl
{
    private Document? _rvtDoc;
    public string Name { get; set; }
    [JsonIgnore] public Wolf? Callback { get; set; }

    public void SendCatchToCallback(Prey c)
    {
        Callback?.Callback?.Push(c);
    }

    public virtual bool Execute()
    {
        try
        {
            // A catch won't handle data retrieval on its own, as it is just meant to be a dumb container.
            // Any data retrieval operation should be done here.
            // If, for example, the result returns a void or a bool itself (without having to get data itself)
            // Just return true. No need to forge a blank Catch. The Direwolf *should* expect this result.
            return Callback is not null; // hunt successful!
        }
        catch
        {
            return false; // failed to hunt.
        }
    }

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

    public override string ToString()
    {
        var d = new Dictionary<string, object>
        {
            { "Name", Name }, { "CreatedAt", DateTime.Now.ToString() }
        };
        return new Prey(d).ToString();
    }
}