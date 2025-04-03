using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Revit.Definitions;

namespace Direwolf.Revit.Howls;

public record class GetDocumentIntrospection : RevitHowl
{
    public GetDocumentIntrospection(Document doc)
    {
        SetRevitDocument(doc);
    }

    public override bool Execute()
    {
        try
        {
            SendCatchToCallback(new Prey(new DocumentIntrospection(GetRevitDocument())));
            SendCatchToCallback(new Prey(new ProjectInformationIntrospection(GetRevitDocument())));
            SendCatchToCallback(new Prey(new ProjectSiteIntrospection(GetRevitDocument())));
            SendCatchToCallback(new Prey(new UnitIntrospection(GetRevitDocument())));
            return true;
        }
        catch
        {
            return false;
        }
    }
}