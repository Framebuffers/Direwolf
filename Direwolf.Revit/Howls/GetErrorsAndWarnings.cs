using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls;

public record GetErrorsAndWarnings : RevitHowl
{
    public GetErrorsAndWarnings(Document doc)
    {
        SetRevitDocument(doc);
    }

    public override bool Hunt()
    {
        Dictionary<string, string> failures = [];
        foreach (var failureMessage in GetRevitDocument().GetWarnings())
            failures[failureMessage.GetDescriptionText() + " " + Guid.NewGuid()] =
                failureMessage.GetSeverity().ToString();

        var d = new Dictionary<string, object>
        {
            ["errorsAndWarnings"] = failures
        };
        SendCatchToCallback(new Prey(d));
        return true;
    }
}