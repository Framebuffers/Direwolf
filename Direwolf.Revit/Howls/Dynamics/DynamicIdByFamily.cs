using Autodesk.Revit.DB;
using Direwolf.Definitions.Dynamics;
using System.Dynamic;

namespace Direwolf.Revit.Howls.Dynamics
{
    public record class DynamicIdByFamily : DynamicRevitHowl
    {
        public DynamicIdByFamily(Document rvt) => SetRevitDocument(rvt);

        public override bool Execute()
        {
            try
            {
                ICollection<Element> allValidElements = new FilteredElementCollector(GetRevitDocument())
                    .WhereElementIsNotElementType()
                    .WhereElementIsViewIndependent()
                    .ToElements();

                dynamic elementsSortedByFamily = new ExpandoObject();

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
                    value?.Add(e.Id.Value);
                }
                SendCatchToCallback(new DynamicCatch(elementsSortedByFamily));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
