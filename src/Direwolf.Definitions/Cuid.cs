using System.Text.Json.Serialization;
using Direwolf.Definitions.Serialization;

namespace Direwolf.Definitions;

/// <summary>
///     A Collision-Resistant Unique Identifier is a lighter type of unique identifier, aimed to offer better
///     performance and readability than a traditional <see cref="Guid" />. It is designed to be sequentially-generated,
///     stores a timestamp of its generation within the string, whilst remaining resistant to two identical values
///     being generated.
///     This type of identifier is used inside <see cref="Direwolf" /> to identify Transactions and RevitElements.
///     <see cref="CuidDriver" /> is responsible for the generation of these identifiers.
///     For more information, visit:
///     <![CDATA[https://www.usefulids.com/resources/what-is-a-cuid]]>
/// </summary>
/// <param name="Length">The length of the randomly-generated part of the identifier.</param>
public readonly record struct Cuid()
{
    
    public static long GetLength(Cuid b) => b.Value!.Length + b.Timestamp!.Length + b.Counter!.Length + b.Fingerprint!.Length + b.Random!.Length + 1;
    [JsonPropertyName("id")]public string? Value { get; init; } = null;
    [JsonIgnore] public long? TimestampNumeric { get; init; } = null;
    [JsonIgnore] public string? Timestamp { get; init; } = null;
    [JsonIgnore] public string? Counter { get; init; } = null;
    [JsonIgnore] public string? Fingerprint { get; init; } = null;
    [JsonIgnore] public string? Random { get; init; } = null;

    /// <summary>
    /// A Wolfpack 1.0 CUID is based on CUID1 and is 32-characters long.
    /// </summary>
    /// <returns>A 32-digit long CUID</returns>
    public static Cuid Create()
    {
        var b = CuidDriver.GenerateDeconstructedCuid();
        
        return new Cuid
        {
            Value = b.Value,
            TimestampNumeric = b.TimeGenerated,
            Timestamp = b.Timestamp,
            Counter = b.Counter,
            Fingerprint = b.Fingerprint,
            Random = b.Random
        };
        
        // var cuid = CuidDriver.GenerateCuid();
        // var s = cuid.Value;
        // Span<Range> random = [20..]; // in case longer CUID's are needed on a future format
        // var timestamp = s!.Substring(1, 7);
        // return new Cuid
        // {
        //     TimestampNumeric = CuidDriver.DecodeBase36(timestamp),
        //     Timestamp = timestamp,
        //     Counter = s.Substring(8, 8),
        //     Fingerprint = s.Substring(16, 16),
        //     Random = random.ToString(),
        //     Value = s
        // };
    }
    

  

    public override string ToString()
    {
        return Value ?? string.Empty;
    }

}