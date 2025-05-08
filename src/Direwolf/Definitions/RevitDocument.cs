using Autodesk.Revit.DB;

namespace Direwolf.Definitions;

public readonly record struct RevitDocument
{
    public string DocumentName { get; init; }
    public string DocumentUniqueId { get; init; }
    public DocumentType DocumentType { get; init; }
}