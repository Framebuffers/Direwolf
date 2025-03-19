using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls
{

    public record class GetAnnotativeElements : RevitHowl
    {
        public GetAnnotativeElements(Document doc) => SetRevitDocument(doc);
        
        public override bool Execute()
        {
            using FilteredElementCollector collector = new(GetRevitDocument());
            ICollection<Element> annotativeElements = collector
                .WhereElementIsNotElementType()
                .Where(e => e.Category != null && e.Category.CategoryType == CategoryType.Annotation)
                .ToList();


            var results = new List<string>();
            foreach (Element element in annotativeElements)
            {
                results.Add(element.Id.Value.ToString());
            }

            var c = new Dictionary<string, object>()
            {
                ["annotativeElements"] = results
            };

            SendCatchToCallback(new Prey(c));
            return true;
        }
    }
}
