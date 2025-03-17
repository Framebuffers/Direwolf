using Autodesk.Revit.DB;

namespace Direwolf.Revit.Howls
{
    public record class GetLargestFamilies : RevitHowl
    {
        public GetLargestFamilies(Document doc) => SetRevitDocument(doc);
        public override bool Execute()
        {
            
            //var d = new Dictionary<string, object>()
            //{

            //};
            //SendCatchToCallback(new Prey(d));
            return true;


        }

    }
}
