using Direwolf.Drivers;

namespace Direwolf.Dto.Parser;

public readonly record struct Cuid(int Length = 4)
{
    public string? Value                 {get; init;} = null;
    public long?   TimestampMilliseconds {get; init;} = null;
    public string? TimestampSubstring    {get; init;} = null;
    public string? CounterSubstring      {get; init;} = null;
    public string? FingerprintSubstring  {get; init;} = null;
    public string? RandomSubstring       {get; init;} = null;

    public static Cuid Create(int length = 4)
    {
        (string Timestamp, string Counter, string Fingerprint, string Random, long TimeGenerated, string Value) cuid
            = CuidDriver.GenerateDeconstructedCuid(length);

        return new Cuid
        {
            Value = cuid.Value,
            TimestampMilliseconds = cuid.TimeGenerated,
            TimestampSubstring = cuid.Timestamp,
            CounterSubstring = cuid.Counter,
            FingerprintSubstring = cuid.Fingerprint,
            RandomSubstring = cuid.Random
        };
    }

    public override string ToString()
    {
        return Value ?? string.Empty;
    }
}