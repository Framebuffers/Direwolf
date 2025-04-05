using System.Diagnostics;
using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Revit.Definitions;

namespace Direwolf.Revit.Howls;

public record GetModelHealth : RevitHowl
{
    public override bool Execute()
    {
        try
        {
            Debug.Print($"Executing RevitHowl on document {GetRevitDocument().Title}");
            SendCatchToCallback(new Prey(new DocumentIntrospection(GetRevitDocument())));
            return true;
        }
        catch (Exception e)
        {
            SendCatchToCallback(new Prey($"Exception thrown. Message:\n{e.Message}"));
            return false;
        }
    }
}