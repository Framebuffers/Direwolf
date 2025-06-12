using Autodesk.Revit.DB;
using Direwolf.Definitions.Internal.Enums;
using Direwolf.Definitions.Parsers;

namespace Direwolf.Definitions.Internal;

public readonly record struct PayloadId(
    Cuid InternalId,
    DataType DataType,
    string? MimeType,
    Dictionary<string, object>? Annotations)
{
    public static PayloadId Create(DataType dataType, string mimeType, Dictionary<string, object>? annotations = null)
    {
        return new PayloadId(Cuid.Create(), dataType, mimeType, annotations);
    }

    public static PayloadId Create()
    {
        return new PayloadId(Cuid.Create(), DataType.Invalid, null, null);
    }
    
    public bool Equals(PayloadId? obj)
    {
        return (obj is { } payloadId && payloadId.InternalId.Equals(InternalId));
    }
}