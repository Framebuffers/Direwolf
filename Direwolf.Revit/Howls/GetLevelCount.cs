using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls
{
    public record class GetLevelCount : RevitHowl
    {
        public GetLevelCount(Document doc) => SetRevitDocument(doc);
        public override bool Execute()
        {
            var d = new Dictionary<string, object>()
            {
                ["levels"] = new FilteredElementCollector(GetRevitDocument())
                .OfClass(typeof(Level))
                .GetElementCount()
            };
            SendCatchToCallback(new Prey(d));
            return true;


        }            
 
    }
}
