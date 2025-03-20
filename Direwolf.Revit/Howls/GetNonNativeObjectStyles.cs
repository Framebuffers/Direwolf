using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls
{
    public record class GetNonNativeObjectStyles : RevitHowl
    {
        public GetNonNativeObjectStyles(Document doc) => SetRevitDocument(doc);

        public override bool Execute()
        {

            using FilteredElementCollector collector = new FilteredElementCollector(GetRevitDocument())
                .OfClass(typeof(GraphicsStyle));

            List<string> nonNativeObjectStyles = [];

            foreach (Element element in collector)
            {
                if (element is GraphicsStyle graphicsStyle)
                {
                    using Category category = graphicsStyle.GraphicsStyleCategory;
                    if (category != null && category.IsCuttable == false && category.CategoryType == CategoryType.Annotation)
                    {
                        nonNativeObjectStyles.Add($"Style Name: {graphicsStyle.Name}, category: {category.Name}");
                    }
                }
            }

            var d = new Dictionary<string, object>()
            {
                ["nonNativeObjectStyles"] = nonNativeObjectStyles
            };
            SendCatchToCallback(new Prey(d));
            return true;


        }
    }
}
