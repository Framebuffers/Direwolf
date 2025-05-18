using System.Security.Cryptography;
using System.Text;
using Direwolf.Dto.Parser;

namespace Direwolf.Drivers;

//TODO: Implement Read/Update/Delete methods, and implement IDriver interface.
public static class CuidDriver
{
    // Adapted from https://www.usefulids.com/resources/generate-cuid-in-csharp
    //      - I updated the code a bit to update deprecated RNG methods.
    //      - Made a new record struct to hold the generated values.
    //          - This enables easy deconstruction of all parts.
    //      - Made the length of the generated value a parameter.
    private static readonly char[] Base36Chars = "0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray();
    private static readonly object LockObject = new();
    private static long _lastTimeStamp = 0;
    private static int _counter = 0;

    public static Cuid GenerateCuid(int length = 4)
    {
        var timestamp = GetCurrentTimeStamp();
        var counter = GetNextCounter(timestamp);

        var timestampPart = EncodeBase36(timestamp);
        var counterPart = EncodeBase36(counter);
        var fingerprintPart = GetMachineFingerprint();
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
        Value) GenerateDeconstructedCuid(int length = 4)
    {
        var timestamp = GetCurrentTimeStamp();
        var counter = GetNextCounter(timestamp);

        var timestampPart = EncodeBase36(timestamp);
        var counterPart = EncodeBase36(counter);
        var fingerprintPart = GetMachineFingerprint();
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

    private static string EncodeBase36(long value)
    {
        var result = new StringBuilder();
        while (value > 0)
        {
            result.Insert(0, Base36Chars[value % 36]);
            value /= 36;
        }

        return result.ToString().PadLeft(8, '0');
    }

    private static string GetMachineFingerprint()
    {
        var machineName = Environment.MachineName;
        var hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(machineName));

        var sb = new StringBuilder();
        foreach (var b in hashBytes)
        {
            sb.Append(Base36Chars[b % 36]);
        }

        return sb.ToString()[..4];
    }

    private static string GetRandomString(int length)
    {
        var data = RandomNumberGenerator.GetBytes(length);
        var sb = new StringBuilder(length);
        foreach (var b in data)
        {
            sb.Append(Base36Chars[b % 36]);
        }

        return sb.ToString();
    }
}