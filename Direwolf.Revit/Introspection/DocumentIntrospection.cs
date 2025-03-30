using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Direwolf.Definitions;
using Direwolf.Revit.Howls;
using Direwolf.Revit.Definitions;

namespace Direwolf.Revit.Introspection
{
    /// <summary>
    /// </summary>
    public record class DocumentIntrospection : RevitHowl
    {
        public DocumentIntrospection(Document doc)
        {
            SetRevitDocument(doc);
        }

        public override bool Execute()
        {
            try
            {

                ProjectInformationIntrospection pi = new(GetRevitDocument());
                ProjectSiteIntrospection ps = new(GetRevitDocument());
                ProjectUnitsIntrospection ui = new(GetRevitDocument());

                SendCatchToCallback(new Prey(pi));
                SendCatchToCallback(new Prey(ui));
                SendCatchToCallback(new Prey(ps));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
