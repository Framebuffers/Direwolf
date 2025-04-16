using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions.Primitives;

public readonly record struct RevitWarnings
{
    public RevitWarnings(Document doc)
    {
        DocumentEpisode = new RevitDocumentEpisode(doc);
        List<string> none = [];
        List<string> warnings = [];
        List<string> errors = [];
        List<string> corruptions = [];

        foreach (var (severity, message) in from w in doc.GetWarnings()
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

        NoSeverity = none;
        Warnings = warnings;
        Errors = errors;
        DocumentCorruption = corruptions;
    }

    public RevitDocumentEpisode DocumentEpisode { get; init; }
    public List<string> NoSeverity { get; init; }
    public List<string> Warnings { get; init; }
    public List<string> Errors { get; init; }
    public List<string> DocumentCorruption { get; init; }
}