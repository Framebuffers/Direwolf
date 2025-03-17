using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls
{
    public record class GetViews : RevitHowl
    {
        public GetViews(Document doc) => SetRevitDocument(doc);
        public override bool Execute()
        {
            using FilteredElementCollector viewCollector = new FilteredElementCollector(GetRevitDocument())
                            .OfClass(typeof(View))
                            .WhereElementIsNotElementType();

            List<string> views = [];

            foreach (Element elem in viewCollector)
            {
                if (elem is View view && !view.IsTemplate)
                {
                    views.Add(view.Name);
                }
            }

            var d = new Dictionary<string, object>()
            {
                ["views"] = views
            };
            SendCatchToCallback(new Prey(d));
            return true;
        }
    }
}
