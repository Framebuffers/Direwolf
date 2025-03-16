using Autodesk.Revit.DB;
using Direwolf.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Howls
{
    public record class GetFamiliesWithMostTypes : RevitHowl
    {
        public GetFamiliesWithMostTypes(Document doc) => SetRevitDocument(doc);
        public override bool Execute()
        {
            using FilteredElementCollector familyCollector = new FilteredElementCollector(doc)
                  .OfClass(typeof(Family));

            List<string> results = [];

            results.AddRange(from Family family in familyCollector
                                   let typeCount = family.GetFamilySymbolIds().Count
                                   where typeCount > 50
                                   select $"Family Name: {family.Name}, Types: {typeCount}");

            var d = new Dictionary<string, object>()
            {
                ["inPlaceFamilies"] = results
            };
            SendCatchToCallback(new Prey(d));
            return true;


        }
    }
}
