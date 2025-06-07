using Autodesk.Revit.DB;

namespace Direwolf.Definitions.Extensions;

public static class ElementIdExtensions
{
     public static string GetUniqueId(this ElementId id, Document doc) => doc.GetElement(id).UniqueId;
}