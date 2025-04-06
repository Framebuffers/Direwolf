using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls;

public record GetDocumentTitle(Document Doc) : RevitHowl
{
    public override bool Hunt()
    {
        var rvtdoc = Doc;
        var data = new Dictionary<string, object>
        {
            ["Title"] = rvtdoc.Title
        };
        // SendCatchToCallback(new Prey(data));
        return true;
    }
}