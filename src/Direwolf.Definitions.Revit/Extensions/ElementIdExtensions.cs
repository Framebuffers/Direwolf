using Autodesk.Revit.DB;

namespace Direwolf.Definitions.PlatformSpecific.Extensions;

public static class ElementIdExtensions
{
    public static string GetUniqueId(this ElementId id, Document doc)
    {
        return doc.GetElement(id).UniqueId;
    }
}