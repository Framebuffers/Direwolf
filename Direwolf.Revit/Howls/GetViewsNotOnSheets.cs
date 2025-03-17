using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls
{
    public record class GetViewsNotOnSheets : RevitHowl
    {
        public GetViewsNotOnSheets(Document doc) => SetRevitDocument(doc);
        public override bool Execute()
        {
            using FilteredElementCollector viewCollector = new FilteredElementCollector(GetRevitDocument())
                            .OfClass(typeof(View))
                            .WhereElementIsNotElementType();

            FilteredElementCollector viewportCollector = new FilteredElementCollector(GetRevitDocument())
                .OfClass(typeof(Viewport));

            HashSet<ElementId> viewsOnSheets = [.. viewportCollector.Select(vp => (vp as Viewport).ViewId)];

            List<string> viewsNotOnSheets = [];

            foreach (Element viewElement in viewCollector)
            {
                if (viewElement is View view && !view.IsTemplate && !viewsOnSheets.Contains(view.Id))
                {
                    viewsNotOnSheets.Add($"View Name: {view.Name}, View ID: {view.Id}");
                }
            }

            var d = new Dictionary<string, object>()
            {
                ["viewsNotOnSheets"] = viewsNotOnSheets
            };
            SendCatchToCallback(new Prey(d));
            return true;


        }
    }
}
