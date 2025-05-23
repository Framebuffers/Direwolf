using System.Runtime.Caching;

using Autodesk.Revit.DB;

namespace Direwolf.Definitions.Internal;

public record Token(Document Document, ObjectCache Cache);