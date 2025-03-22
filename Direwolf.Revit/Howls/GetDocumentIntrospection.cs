using Autodesk.Revit.DB;
using Direwolf.Revit.Definitions;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls
{
    public record class GetDocumentIntrospection : RevitHowl
    {
        public GetDocumentIntrospection(Document doc)
        {
            ArgumentNullException.ThrowIfNull(doc);
            SetRevitDocument(doc);
        }

        public override bool Execute()
        {
            try
            {
                DocumentIntrospection di = new(GetRevitDocument());
                SendCatchToCallback(new Prey(di));
                return true;
            }
            catch (Exception e)
            {
                SendCatchToCallback(new Prey($"Exception caught: {e.Message}"));
                return false;
            }
        }
    }
}
