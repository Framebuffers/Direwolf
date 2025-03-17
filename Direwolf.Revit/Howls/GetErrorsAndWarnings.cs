using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls
{
    public record class GetErrorsAndWarnings : RevitHowl
    {
        public GetErrorsAndWarnings(Document doc) => SetRevitDocument(doc);
        public override bool Execute()
        {
            Dictionary<string, string> failures = [];
            foreach (FailureMessage failureMessage in GetRevitDocument().GetWarnings())
            {
                failures[failureMessage.GetDescriptionText() + " " + Guid.NewGuid()] = failureMessage.GetSeverity().ToString();
            }

            var d = new Dictionary<string, object>()
            {
                ["errorsAndWarnings"] = failures
            };
            SendCatchToCallback(new Prey(d));
            return true;


        }
    }
}
