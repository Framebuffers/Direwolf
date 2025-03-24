using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls
{
    public record class GetFamilyIntrospection : RevitHowl
    {
        public GetFamilyIntrospection(Document doc, Family? f)
        {
            ArgumentNullException.ThrowIfNull(doc);
            ArgumentNullException.ThrowIfNull(f);
            SetRevitDocument(doc);
            _f = f;
        }
        private Family? _f;
        public override bool Execute()
        {
            try
            {
                //if (_f is not null)
                //    SendCatchToCallback(new Prey(new FamilyIntrospection(_f)));
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
