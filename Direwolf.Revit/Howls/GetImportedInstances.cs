using Autodesk.Revit.DB;
using Direwolf.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Howls
{
    public record class GetImportedInstances : RevitHowl
    {
        public GetImportedInstances(Document doc) => SetRevitDocument(doc);
        public override bool Execute()
        {
            ICollection<Element> importedImages = new FilteredElementCollector(GetRevitDocument())
                .OfClass(typeof(ImportInstance))
                .WhereElementIsNotElementType()
                .ToList();

            var results = importedImages.Select(element => element.Id.Value.ToString()).ToList();

            var d = new Dictionary<string, object>()
            {
                ["importedImages"] = results
            };
            SendCatchToCallback(new Prey(d));
            return true;


        }
    }
}
