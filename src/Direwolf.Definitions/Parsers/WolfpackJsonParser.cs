using System.Text.Json;
using System.Text.Json.Serialization;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.Internal.Enums;

namespace Direwolf.Definitions.Parsers;

public sealed class WolfpackJsonParser : JsonConverter<Wolfpack>
{
    public override Wolfpack Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader is not { TokenType: JsonTokenType.StartObject }) throw new JsonException(nameof(options));
        while (reader.Read())
        {
            Cuid c = new();
            Method? m = null;
            string? name = null;
            Dictionary<string, object>? properties = null;
            List<Howl>? payload = null;

            var propertyName = reader.GetString();
            switch (reader)
            {
                case { TokenType: JsonTokenType.EndObject }:
                    return new Wolfpack(c, m, name, properties?.AsReadOnly(), payload?.AsReadOnly());
                case { TokenType: JsonTokenType.PropertyName }:
                    reader.Read();
                    switch (propertyName)
                    {
                        case "Id":
                            if (string.IsNullOrEmpty(reader.GetString()))
                                throw new JsonException($"{nameof(Cuid)} in Id is empty.");
                            c = reader.GetString()!.ParseAsCuid();
                            break;
                        case "Method":
                            if (string.IsNullOrEmpty(reader.GetString()))
                                throw new JsonException($"{nameof(Method)} is empty.");
                            if (!Enum.TryParse<Method>(reader.GetString()!, true, out var method))
                                throw new JsonException($"{nameof(Method)} is not a valid method.");
                            m = method;
                            break;
                        case "Name":
                            if (string.IsNullOrEmpty(reader.GetString()))
                                throw new JsonException($"{nameof(name)} is empty.");
                            name = reader.GetString();
                            break;

                        case "Parameters":
                            if (string.IsNullOrEmpty(reader.GetString()))
                                throw new JsonException($"{nameof(properties)} is empty.");
                            if (reader.TokenType == JsonTokenType.StartObject)
                                while (reader.Read() && reader is not { TokenType: JsonTokenType.EndObject })
                                {
                                    properties ??= [];
                                    if (string.IsNullOrEmpty(reader.GetString()))
                                        continue;
                                    var key = reader.GetString();
                                    reader.Read();
                                    var value = reader.GetString();

                                    if (key is null || value is null) continue;
                                    properties[key] = value;
                                }

                            break;
                        case "Data":
                            if (string.IsNullOrEmpty(reader.GetString()))
                                throw new JsonException(" is empty.");
                            if (reader.TokenType == JsonTokenType.StartArray)
                                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                                {
                                    var howl = JsonSerializer.Deserialize<Howl>(ref reader, options);
                                    payload?.Add(howl);
                                }

                            break;
                        default:
                            reader.Skip();
                            break;
                    }

                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        throw new JsonException("Unexpected end of JSON.");
    }


    public override void Write(Utf8JsonWriter writer, Wolfpack value, JsonSerializerOptions options)
    {
        // unimplemented
    }
}