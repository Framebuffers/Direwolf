using Autodesk.Revit.DB;

namespace Direwolf.Dto.RevitApi;

public readonly record struct DocumentWarnings(
    List<string> NoSeverityWarningMessages,
    List<string> WarningMessages,
    List<string> ErrorMessages,
    List<string> DocumentCorruptionMessages)
{
    public static DocumentWarnings Create(Document doc)
    {
        List<string> none = [];
        List<string> warnings = [];
        List<string> errors = [];
        List<string> corruptions = [];

        foreach ((var severity, string? message) in
                 from w in doc.GetWarnings()
                 let s = w.GetSeverity()
                 let m = w.GetDescriptionText()
                 select (s, m))
            switch (severity)
            {
                case FailureSeverity.None:
                    none.Add(message);

                    break;
                case FailureSeverity.Warning:
                    warnings.Add(message);

                    break;
                case FailureSeverity.Error:
                    errors.Add(message);

                    break;
                case FailureSeverity.DocumentCorruption:
                    corruptions.Add(message);

                    break;
            }

        return new DocumentWarnings(none,
                                    warnings,
                                    errors,
                                    corruptions);
    }
}