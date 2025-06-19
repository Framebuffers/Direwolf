using System.Security.Cryptography;
using System.Text;
using Autodesk.Revit.DB;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.Serialization;

namespace Direwolf.Definitions.PlatformSpecific.Extensions;

public static class DirewolfRvtExtensions
{
    public static string GetDocumentUuidHash(this Document doc)
    {
        var base36Chars = CuidExtensions.Base36Chars.ToCharArray();
        var docUniqueId = doc.CreationGUID;
        var hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(docUniqueId.ToString()));
        var sb = new StringBuilder();
        foreach (var b in hashBytes) sb.Append(base36Chars[b % 36]);
        Span<char> buffer = stackalloc char[4];
        for (var i = 0; i < buffer.Length; i++) buffer[i] = CuidExtensions.Base36Chars[hashBytes[i] % 36];
        return buffer.ToString();
    }

    private static string GetDocumentVersionCounter(this Document doc)
    {
        return CuidDriver.EncodeBase36(Document.GetDocumentVersion(doc).NumberOfSaves);
    }

    public static string GetDocumentVersionHash(this Document doc)
    {
        return string.Concat(GetDocumentVersionCounter(doc), GetDocumentUuidHash(doc));
    }
}