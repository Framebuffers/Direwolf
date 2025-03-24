using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls
{
    public record class GetProjectInformationIntrospection : RevitHowl
    {
        public GetProjectInformationIntrospection(Document doc)
        {
            ArgumentNullException.ThrowIfNull(doc);
            SetRevitDocument(doc);
        }

        public override bool Execute()
        {
            try
            {
                //SendCatchToCallback(new Prey(new ProjectInformationIntrospection(GetRevitDocument())));
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
