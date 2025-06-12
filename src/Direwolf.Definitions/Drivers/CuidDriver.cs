using System.Security.Cryptography;
using System.Text;
using Autodesk.Revit.DB;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.Parsers;

namespace Direwolf.Definitions.Drivers;

//TODO: Implement Read/Update/Delete methods, and implement IDriver interface.
public static partial class CuidDriver
{
    // Adapted from https://www.usefulids.com/resources/generate-cuid-in-csharp
    //      - I updated the code a bit to update deprecated RNG methods.
    //      - Made a new record struct to hold the generated values.
    //          - This enables easy deconstruction of all parts.
    //      - Made the length of the generated value a parameter.
    private static readonly char[] Base36Chars = "0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray();
    private static readonly object LockObject = new();
    private static long _lastTimeStamp;
    private static int _counter;

    public static Cuid GenerateCuid(int length = 16)
    {
        var timestamp = GetCurrentTimeStamp();
        var counter = GetNextCounter(timestamp);
        var timestampPart = EncodeBase36(timestamp);
        var counterPart = EncodeBase36(counter);
        var fingerprintPart = GenerateFingerprint(Environment.MachineName);
        var randomPart = GetRandomString(length);
        var value = $"c{timestampPart}{counterPart}{fingerprintPart}{randomPart}";
        return new Cuid(length)
        {
            Value = value,
            TimestampMilliseconds = timestamp,
            TimestampSubstring = timestampPart,
            CounterSubstring = counterPart,
            FingerprintSubstring = fingerprintPart,
            RandomSubstring = randomPart
        };
    }

    public static ( string Timestamp, string Counter, string Fingerprint, string Random, long TimeGenerated, string
        Value ) GenerateDeconstructedCuid(int length = 4)
    {
        var timestamp = GetCurrentTimeStamp();
        var counter = GetNextCounter(timestamp);
        var timestampPart = EncodeBase36(timestamp);
        var counterPart = EncodeBase36(counter);
        var fingerprintPart = GenerateFingerprint(Environment.MachineName);
        var randomPart = GetRandomString(length);
        var value = $"c{timestampPart}{counterPart}{fingerprintPart}{randomPart}";
        return (timestampPart, counterPart, fingerprintPart, randomPart, timestamp, value);
    }

    private static long GetCurrentTimeStamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private static int GetNextCounter(long timestamp)
    {
        lock (LockObject)
        {
            if (timestamp == _lastTimeStamp) return ++_counter;
            _lastTimeStamp = timestamp;
            _counter = 0;
            return _counter;
        }
    }

    internal static string EncodeBase36(long value)
    {
        var result = new StringBuilder();
        while (value > 0)
        {
            result.Insert(0, Base36Chars[value % 36]);
            value /= 36;
        }

        return result.ToString().PadLeft(8, '0');
    }

    public static long DecodeBase36(string base36)
    {
        const string chars = "0123456789abcdefghijklmnopqrstuvwxyz";
        return base36.Aggregate<char, long>(0, (current, c) => current * 36 + chars.IndexOf(c));
    }

    private static string GenerateFingerprint(string machineName)
    {
        var hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(machineName));
        var sb = new StringBuilder();
        foreach (var b in hashBytes) sb.Append(Base36Chars[b % 36]);
        return sb.ToString()[..4];
    }

    private static string GetRandomString(int length)
    {
        var data = RandomNumberGenerator.GetBytes(length);
        var sb = new StringBuilder(length);
        foreach (var b in data) sb.Append(Base36Chars[b % 36]);
        return sb.ToString();
    }
}

// Direwolf-specific
public static partial class CuidDriver
{
    /// <summary>
    ///     Gets a specialised <see cref="Cuid" /> to be used inside a Revit context.
    ///     The difference between this and any other <see cref="Cuid" /> is that:
    ///     <list type="bullet">
    ///         <item>The counter is replaced by the save counter of the Revit <see cref="Document" /></item>
    ///         <item>The fingerprint is the CreationGUID of the <see cref="Document" /></item>
    ///     </list>
    ///     This allows Direwolf to track an <see cref="Autodesk.Revit.DB.Element" /> and its origin just by this
    ///     identifier.
    /// </summary>
    /// <param name="doc">Revit Document</param>
    /// <param name="documentIdentifier">
    ///     A tuple with the hashed Base36 truncated string of the <see cref="Document" />
    ///     VersionID and the <see cref="Document.CreationGUID" />
    /// </param>
    /// <param name="length">Length of the random part of the identifier.</param>
    /// <returns>
    ///     A Collision-Resistant Unique Identifier string with its Counter and Fingerprint referencing the given
    ///     <see cref="Document" />.
    /// </returns>
    public static Cuid NewDirewolfId(Document doc, out (string, string) documentIdentifier, int length = 16)
    {
        var timestamp = GetCurrentTimeStamp();
        var timestampPart = EncodeBase36(timestamp);
        var counterPart = EncodeBase36(Document.GetDocumentVersion(doc).VersionGUID.GetHashCode());
        var fingerprintPart = doc.GetDocumentUuidHash();
        var randomPart = GetRandomString(length);
        var value = $"c{timestampPart}{counterPart}{fingerprintPart}{randomPart}";
        documentIdentifier = (doc.GetDocumentVersionHash(), doc.GetDocumentUuidHash());
        return new Cuid(length)
        {
            Value = value,
            TimestampMilliseconds = timestamp,
            TimestampSubstring = timestampPart,
            CounterSubstring = counterPart,
            FingerprintSubstring = fingerprintPart,
            RandomSubstring = randomPart
        };
    }
}