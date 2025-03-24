using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls
{
    public record class GetProjectSiteIntrospection : RevitHowl
    {
        public GetProjectSiteIntrospection(Document doc)
        {
            ArgumentNullException.ThrowIfNull(doc);
            SetRevitDocument(doc);
        }

        public override bool Execute()
        {
            try
            {
                //SendCatchToCallback(new Prey(new GetProjectSiteIntrospection(GetRevitDocument())));
                return true;
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Exception thrown: {e.Message}"));
                return false;
            }
        }
    }
}
