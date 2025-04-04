using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls;

public record GetModelHealth : RevitHowl
{
    public GetModelHealth(Document doc)
    {
        SetRevitDocument(doc);
    }

    public override bool Execute()
    {
        try
        {
            return true;
        }
        catch (Exception e)
        {
            SendCatchToCallback(new Prey($"Exception thrown. Message:\n{e.Message}"));
            return false;
        }
    }
}