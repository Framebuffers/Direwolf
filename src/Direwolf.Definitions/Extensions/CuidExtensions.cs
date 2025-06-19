using System.Security.Cryptography;
using System.Text;
using Autodesk.Revit.DB;
using Direwolf.Definitions.Serialization;

namespace Direwolf.Definitions.Extensions;

public static class CuidExtensions
{
    internal const string Base36Chars = "0123456789abcdefghijklmnopqrstuvwxyz";

    public static string GetDocumentUuidHash(this Document doc)
    {
        var base36Chars = Base36Chars.ToCharArray();
        var docUniqueId = doc.CreationGUID;
        var hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(docUniqueId.ToString()));
        var sb = new StringBuilder();
        foreach (var b in hashBytes) sb.Append(base36Chars[b % 36]);
        Span<char> buffer = stackalloc char[4];
        for (var i = 0; i < buffer.Length; i++) buffer[i] = Base36Chars[hashBytes[i] % 36];
        return buffer.ToString();
    }

    public static string GetDocumentVersionCounter(this Document doc)
    {
        return CuidDriver.EncodeBase36(Document.GetDocumentVersion(doc).NumberOfSaves);
    }

    public static string GetDocumentVersionHash(this Document doc)
    {
        return string.Concat(GetDocumentVersionCounter(doc), GetDocumentUuidHash(doc));
    }

    /// <summary>
    ///     Takes a string and tries to parse it as a <see cref="Cuid" />.
    ///     <remarks>
    ///         The substring offsets for this <see cref="Cuid" /> are:
    ///         <list type="bullet">
    ///             <item>
    ///                 <term>Timestamp: </term>
    ///                 <description>start = [0], length = 7</description>
    ///             </item>
    ///             <item>
    ///                 <term>Counter: </term>
    ///                 <description>start = [8], length = 8</description>
    ///             </item>
    ///             <item>
    ///                 <term>Fingerprint: </term>
    ///                 <description>start = [16], length = 4</description>
    ///             </item>
    ///             <item>
    ///                 <term>Random: </term>
    ///                 <description>start = [20], length = variable</description>
    ///             </item>
    ///         </list>
    ///     </remarks>
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static Cuid ParseAsCuid(this string s)
    {
        var random = s[20..];
        var timestamp = s.Substring(1, 7);
        return new Cuid
        {
            TimestampMilliseconds = CuidDriver.DecodeBase36(timestamp),
            TimestampSubstring = timestamp,
            CounterSubstring = s.Substring(8, 8),
            FingerprintSubstring = s.Substring(16, 4),
            RandomSubstring = random,
            Length = random.Length,
            Value = s
        };
    }

    public static DateTimeOffset GetDateTimeCreation(this Cuid id) => DateTimeOffset.FromUnixTimeMilliseconds(id.TimestampMilliseconds!.Value);
}