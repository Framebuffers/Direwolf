namespace Direwolf.Revit.Definitions.Primitives;

public readonly record struct RevitPerformance
{
    public RevitDocumentEpisode Episode { get; init; }
    public int ApplicationLoadTime { get; init; }
    public int DocumentLoadTime { get; init; }
    public int ResponseTimeAverage { get; init; }
    public int ViewChangeTimeAverage { get; init; }
    public int DocumentPrintingTimeAverage { get; init; }
    public int DocumentSyncTimeAverage { get; init; }
    public int DocumentSavingTimeAverage { get; init; }
    public int ElementDuplicationTimeAverage { get; init; }
    public int FailureProcessingEvents { get; init; }
    public int FileExportTimeAverage { get; init; }
    public int FileImportTimeAverage { get; init; }
    public int LinkedResourceLoadTimeAverage { get; init; }
    public int ProgressBarTimeAverage { get; init; }
    public int ViewExportTimeAverage { get; init; }
    public int ViewPrintTimeAverage { get; init; }
    public double FileSize { get; init; }
    public int FamilyCount { get; init; }
    public int WarningCount { get; init; }
    public int ViewsInsideDocument { get; init; }
    public int ViewsNotInSheets { get; init; }
    public int AnnotativeElementCount { get; init; }
    public int ExternalReferenceCount { get; init; }
    public int ModelGroupCount { get; init; }
    public int DetailGroupCount { get; init; }
    public int DesignGroupCount { get; init; }
    public int LevelCount { get; init; }
    public int GridCount { get; init; }
    public int UnenclosedRoomCount { get; init; }
    public int ViewportCount { get; init; }
    public int UnconnectedDuctCount { get; init; }
    public int UnconnectedPipeCount { get; init; }
    public int UnconnectedElectricalCount { get; init; }
    public int NonNativeStyleCount { get; init; }
    public int FlippedElementCount { get; init; }
    public int WorksetCount { get; init; }
}