using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls
{
    public record class DocumentTest(Document doc) : RevitHowl
    {
        public override bool Execute()
        {
            var rvtdoc = doc;
            var data = new Dictionary<string, object>()
            {
                ["Title"] = rvtdoc.Title
            };
            SendCatchToCallback(new Prey(data));
            return true;
        }
    }

    
}
