using Autodesk.Revit.DB;

namespace Direwolf.Definitions.Internal;

public readonly record struct DocumentId
{
    public string       DocumentName     {get; init;}
    public string       DocumentUniqueId {get; init;}
    public DocumentType DocumentType     {get; init;}
}