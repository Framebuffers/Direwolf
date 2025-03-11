using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls
{
    public record class GetElementIdByFamily : RevitHowl
    {
        public GetElementIdByFamily(Document rvt) => SetRevitDocument(rvt);

        public override bool Execute()
        {
            try
            {
                ICollection<Element> allValidElements = new FilteredElementCollector(GetRevitDocument())
                    .WhereElementIsNotElementType()
                    .WhereElementIsViewIndependent()
                    .ToElements();

                var elementsSortedByFamily = new Dictionary<string, List<long>>();

                foreach ((Element e, string familyName) in from Element e in allValidElements 
                                                           let f = e as FamilyInstance 
                                                           where f is not null 
                                                           let familyName = f.Symbol.Family.Name 
                                                           select (e, familyName)) 
                {
                    if (!elementsSortedByFamily.TryGetValue(familyName, out List<long>? value))
                    {
                        value = [];
                        elementsSortedByFamily[familyName] = value;
                    }
                    value.Add(e.Id.Value);
                }

                SendCatchToCallback(new Prey(new Dictionary<string, object>()
                {
                    ["ElementsByFamily"] = elementsSortedByFamily
                }));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
