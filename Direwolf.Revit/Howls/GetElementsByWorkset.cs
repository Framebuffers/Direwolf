using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls
{
    public record class GetElementsByWorkset : RevitHowl
    {
        public GetElementsByWorkset(Document doc) => SetRevitDocument(doc);
        public override bool Execute()
        {
            using FilteredElementCollector collector = new FilteredElementCollector(GetRevitDocument())
                            .WhereElementIsNotElementType();

            Dictionary<string, int> worksetElementCount = [];

            foreach (Element element in collector)
            {
                using Parameter worksetParam = element.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);

                if (worksetParam != null)
                {
                    string worksetName = worksetParam.AsValueString();

                    if (worksetElementCount.TryGetValue(worksetName, out int value))
                    {
                        worksetElementCount[worksetName] = value++;
                    }
                    else
                    {
                        worksetElementCount[worksetName] = 1;
                    }
                }
            }

            var d = new Dictionary<string, object>()
            {
                ["elementsByWorkset"] = worksetElementCount
            };
            SendCatchToCallback(new Prey(d));
            return true;


        }
    }
}
