using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Examples.Howls
{
    public record class GetElementIdByFamily(Document RevitDocument) : Howl
    {
        public override bool Execute()
        {
            try
            {
                ICollection<Element> allValidElements = new FilteredElementCollector(RevitDocument)
                    .WhereElementIsNotElementType()
                    .WhereElementIsViewIndependent()
                    .ToElements();

                var elementsSortedByFamily = new Dictionary<string, List<long>>();

                foreach ((Element e, string familyName) in from Element e in allValidElements // create two variables, Element e and string familyType
                                                           let f = e as FamilyInstance // cast each element as a FamilyInstance
                                                           where f is not null // check if it's not null
                                                           let familyName = f.Symbol.Family.Name // assign the family name to the variable familyName
                                                           select (e, familyName)) // get the variables back
                {
                    if (!elementsSortedByFamily.TryGetValue(familyName, out List<long>? value))
                    {
                        value = [];
                        elementsSortedByFamily[familyName] = value;
                    }
                    value.Add(e.Id.Value);
                }

                SendCatchToCallback(new Catch(new Dictionary<string, object>()
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
