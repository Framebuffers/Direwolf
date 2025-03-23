using Autodesk.Revit.DB;
using Direwolf.Revit.Definitions;
using Direwolf.Definitions;
using Direwolf.Revit.Howls.Legacy;

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
            _GetElementInformation
                var doc = GetRevitDocument();
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
