using Autodesk.Revit.DB;
using Direwolf.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Howls
{
    public record class GetFamilyCount : RevitHowl
    {
        public GetFamilyCount(Document doc) => SetRevitDocument(doc);
        public override bool Execute()
        {
            using FilteredElementCollector familyCollector = new(GetRevitDocument());

            Dictionary<string, int> familyCounts = [];

            foreach (Family family in familyCollector)
            {
                ICollection<ElementId> familyTypeIds = family.GetFamilySymbolIds();
                int count = 0;

                foreach (ElementId typeId in familyTypeIds)
                {
                    count += new FilteredElementCollector(GetRevitDocument())
                        .OfCategoryId(family.FamilyCategory.Id)
                        .WhereElementIsNotElementType()
                        .Where(e => e.GetTypeId() == typeId)
                        .ToList()
                        .Count;
                }

                familyCounts[family.Name] = count;
            }

            var d = new Dictionary<string, object>()
            {
                ["familyCount"] = familyCounts
            };
            SendCatchToCallback(new Prey(d));
            return true;


        }
    }
}
