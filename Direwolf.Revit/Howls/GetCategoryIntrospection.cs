using Autodesk.Revit.DB;
using Direwolf.Definitions;
using Direwolf.Revit.Definitions;

namespace Direwolf.Revit.Howls
{
    public record class GetCategoryIntrospection : RevitHowl
    {
        public GetCategoryIntrospection(Document doc, Category? c)
        {
            ArgumentNullException.ThrowIfNull(doc);
            ArgumentNullException.ThrowIfNull(c);
            SetRevitDocument(doc);
            _c = c;
        }

        private Category? _c;
        public override bool Execute()
        {
            try
            {
                if (_c is not null)
                    SendCatchToCallback(new Prey(new CategoryIntrospection(_c)));
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
