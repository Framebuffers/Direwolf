using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls
{
    public record class GetProjectUnitsIntrospection : RevitHowl
    {
        public GetProjectUnitsIntrospection(Document doc)
        {
            ArgumentNullException.ThrowIfNull(doc);
            SetRevitDocument(doc);
        }

        public override bool Execute()
        {
            try
            {
                //SendCatchToCallback(new Prey(new UnitIntrospection(GetRevitDocument())));
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
