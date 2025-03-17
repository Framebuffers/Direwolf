using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls
{
    public record class GetTotalSizeOfFamiliesByMB : RevitHowl
    {
        public GetTotalSizeOfFamiliesByMB(Document doc) => SetRevitDocument(doc);
        public override bool Execute()
        {
            var d = new Dictionary<string, object>()
            {

            };
            SendCatchToCallback(new Prey(d));
            return true;


        }
    }
}
