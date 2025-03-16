using Autodesk.Revit.DB;
using Direwolf.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Howls
{
    public record class GetUnusedFamilies : RevitHowl
    {
        public GetUnusedFamilies(Document doc) => SetRevitDocument(doc);
        public override bool Execute()
        {
            using FilteredElementCollector familyCollector = new FilteredElementCollector(GetRevitDocument())
                            .OfClass(typeof(Family));

            List<string> unusedFamilies = [];

            foreach (Family family in familyCollector.Cast<Family>())
            {
                try
                {
                    using FilteredElementCollector instanceCollector = new FilteredElementCollector(GetRevitDocument())
                        .OfCategory(family.Category.BuiltInCategory)
                        .WhereElementIsNotElementType();

                    if (!instanceCollector.Any())
                    {
                        unusedFamilies.Add(family.Name);
                    }
                }
                catch
                {
                    continue;
                }
            }

            var d = new Dictionary<string, object>()
            {
                ["unusedFamilies"] = unusedFamilies
            };
            SendCatchToCallback(new Prey(d));
            return true;


        }
    }
}
