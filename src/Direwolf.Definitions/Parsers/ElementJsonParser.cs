using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Autodesk.Revit.DB;
using Direwolf.Definitions.Extensions;
using Direwolf.Definitions.RevitApi;
using static System.Enum;

namespace Direwolf.Definitions.Parsers;

[SuppressMessage("ReSharper", "RedundantAssignment")]
public sealed class ElementJsonParser : JsonConverter<RevitElement>
{
    public override bool HandleNull => true;

    // TODO: Test the reade.
    public override RevitElement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.StartObject) throw new JsonException(nameof(options));
        while (reader.Read())
        {
            Cuid id = default;
            CategoryType categoryType = default;
            const BuiltInCategory builtInCategory = default;
            ElementId? elementTypeId = null;
            string? elementUniqueId = null;
            ElementId? elementId = null;
            string? elementName = null;
            IReadOnlyList<RevitParameter?> parameters = [];

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (reader.TokenType)
            {
                case JsonTokenType.EndObject:
                    return new RevitElement(id, categoryType, builtInCategory, elementTypeId,
                        elementUniqueId ?? Guid.Empty.ToString(), elementId, elementName ?? string.Empty, parameters);
                case JsonTokenType.PropertyName:
                {
                    var propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "elementUniqueId":
                            if (string.IsNullOrEmpty(reader.GetString()))
                                throw new JsonException("ElementUniqueId is empty.");
                            elementUniqueId = reader.GetString()!;
                            break;
                        case "id":
                            if (string.IsNullOrEmpty(reader.GetString()) || reader.GetString()?.Length < 20)
                                throw new JsonException("Invalid CUID length. Must be 20 characters or longer.");
                            id = reader.GetString()!.ParseAsCuid();
                            break;
                        case "elementId":
                            if (string.IsNullOrEmpty(reader.GetString()))
                                throw new JsonException("Invalid ElementId value");
                            if (!ElementId.TryParse(reader.GetString(), out elementId))
                                throw new JsonException("Invalid ElementId value: Cannot parse ElementId.");
                            break;
                        case "elementTypeId":
                            if (string.IsNullOrEmpty(reader.GetString()))
                                throw new JsonException("Invalid ElementId value");
                            if (!ElementId.TryParse(reader.GetString(), out elementTypeId))
                                throw new JsonException($"CategoryType is invalid: {elementTypeId}");
                            break;
                        case "elementName":
                            elementName = reader.GetString() ?? string.Empty;
                            break;
                        case "categoryType":
                            var categoryString = reader.GetString();
                            if (string.IsNullOrEmpty(categoryString)) throw new JsonException("CategoryType is empty.");
                            if (!TryParse(categoryString, out categoryType))
                                throw new JsonException($"CategoryType is invalid: {categoryString}");
                            break;
                        case "builtInCategory":
                            var bic = reader.GetString();
                            if (string.IsNullOrEmpty(bic)) throw new JsonException("BuiltInCategory is empty.");
                            if (!TryParse(bic, out categoryType))
                                throw new JsonException($"BuiltInCategory is invalid: {bic}");
                            break;
                        case "parameters":
                            parameters = ReadParameters(ref reader);
                            break;
                    }

                    break;
                }
                default:
                    reader.Skip();
                    break;
            }
        }

        throw new JsonException("Unexpected end of JSON.");
    }

    private static ImmutableList<RevitParameter?> ReadParameters(ref Utf8JsonReader reader)
    {
        var parameters = ImmutableList.CreateBuilder<RevitParameter?>();
        if (reader.TokenType is not JsonTokenType.StartArray) throw new JsonException("Expected StartArray token");
        while (reader.Read() && reader.TokenType is not JsonTokenType.EndArray)
        {
            if (reader.TokenType is not JsonTokenType.StartObject) continue;
            string key = null!;
            string value = null!;
            StorageType storageType = default;
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    if (string.IsNullOrEmpty(reader.GetString())) continue;
                    var propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "key":
                            key = (string.IsNullOrEmpty(reader.GetString()) ? reader.GetString() : "undefined") ??
                                  "undefined";
                            break;
                        case "value":
                            value = (string.IsNullOrEmpty(reader.GetString()) ? reader.GetString() : "undefined") ??
                                    "undefined";
                            break;
                        case "storageType":
                            storageType = TryParse<StorageType>(reader.GetString(), out var st) ? st : StorageType.None;
                            break;
                    }
                }

            parameters.Add(new RevitParameter { StorageType = storageType, Key = key, Value = value });
        }

        return parameters.ToImmutableList();
    }

    public override void Write(Utf8JsonWriter writer, RevitElement value, JsonSerializerOptions options)
    {
        // {
        writer.WriteStartObject(value.ElementUniqueId);

        //      "id": "<cuid>",
        writer.WritePropertyName(nameof(value.Id));
        writer.WriteStringValue(value.Id.Value);

        //      "elementId": 123456,
        writer.WritePropertyName(nameof(value.ElementId));
        writer.WriteNumberValue(value.ElementId?.Value ?? -1);

        //      "elementTypeId": 123456,
        writer.WritePropertyName(nameof(value.ElementTypeId));
        writer.WriteNumberValue(value.ElementTypeId?.Value ?? -1);

        //      "elementName": "name"
        writer.WritePropertyName(nameof(value.ElementName));
        writer.WriteStringValue(value.ElementName ?? string.Empty);

        //  {
        //      "categoryType": "type",
        writer.WritePropertyName(nameof(CategoryType));
        writer.WriteStringValue(value.CategoryType.ToString());

        //      "builtInCategory": ""BuiltInCategory.OST_Category"
        writer.WritePropertyName(nameof(BuiltInCategory));
        writer.WriteStringValue(value.BuiltInCategory.ToString());

        //      "parameters": [
        writer.WriteStartArray(nameof(value.Parameters));
        foreach (var parameter in value.Parameters) WriteParameter(writer, parameter, options);
        // ]}
        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    private static void WriteParameter(Utf8JsonWriter writer, RevitParameter? parameter, JsonSerializerOptions options)
    {
        _ = options;

        // {
        //   "<Definition.ArgumentName>": "<Value>",
        //   "storageType": "StorageType.JsonType"
        //  },
        //
        // or
        //
        // {
        //   "undefined": "undefined"
        //   "storageType": "undefined"
        // },
        if (parameter is null) return;
        writer.WriteStartObject(string.IsNullOrWhiteSpace(parameter.Value.Key) ? "undefined" : parameter.Value.Key);
        writer.WritePropertyName(nameof(parameter.Value));
        writer.WriteStringValue(string.IsNullOrWhiteSpace(parameter.Value.Value) ? "undefined" : parameter.Value.Value);
        writer.WritePropertyName(nameof(parameter.Value.StorageType));
        writer.WriteStringValue(parameter.Value.StorageType.ToString());
        writer.WriteEndObject();
    }
}