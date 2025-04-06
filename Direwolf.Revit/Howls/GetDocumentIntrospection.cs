using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Revit.Definitions;

namespace Direwolf.Revit.Howls;

public record GetDocumentIntrospection : RevitHowl
{
    public GetDocumentIntrospection(Document doc)
    {
        SetRevitDocument(doc);
    }

    public override bool Hunt()
    {
        try
        {
            SendCatchToCallback(new Prey(DocumentMetadataWolfpack.CreateInstance(GetRevitDocument())));
            SendCatchToCallback(new Prey(new ProjectInformationIntrospection(GetRevitDocument())));
            SendCatchToCallback(new Prey(new ProjectSiteIntrospection(GetRevitDocument())));
            SendCatchToCallback(new Prey(DocumentUnitWolfpack.CreateInstance(GetRevitDocument())));
            return true;
        }
        catch
        {
            return false;
        }
    }
}