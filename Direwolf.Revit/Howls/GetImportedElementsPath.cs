using Autodesk.Revit.DB;
using Direwolf.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Howls
{
    public record class GetImportedElementsPath : RevitHowl
    {
        public GetImportedElementsPath(Document doc) => SetRevitDocument(doc);
        public override bool Execute()
        {
            using FilteredElementCollector filteredElementCollector = new(GetRevitDocument());
            ICollection<Element> importedElements = filteredElementCollector
                .OfClass(typeof(ImportInstance))
                .WhereElementIsNotElementType()
                .ToList();

            var results = new Dictionary<string, string>();
            foreach (Element element in importedElements)
            {
                using Parameter pathParam = element.get_Parameter(BuiltInParameter.IMPORT_SYMBOL_NAME);
                string filePath = pathParam != null ? pathParam.AsString() : "Unknown Path";
                results[element.UniqueId] = filePath;
            }

            var d = new Dictionary<string, object>()
            {
                ["importedElements"] = results
            };
            SendCatchToCallback(new Prey(d));
            return true;


        }
    }
}
