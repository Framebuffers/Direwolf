namespace Direwolf.Definitions.Parser;

public static class RevitElementJsonSchema
{
  /// <summary>
  ///     Use this to check whether a RevitElement is being converted correctly.
  ///     TODO: sort out the capitalization of the keys.
  /// </summary>
  // ReSharper disable once UnusedMember.Global
#pragma warning disable CA2211
    public const string JsonSchema = """
                                     {
                                       "$schema": "http://json-schema.org/draft-07/schema#",
                                       "type": "object",
                                       "properties": {
                                         "elementUniqueId": {
                                           "type": "string",
                                           "format": "uuid",
                                           "description": "The ElementUniqueId property inside the Element Revit object."
                                         },
                                         "id": {
                                           "type": "string",
                                           "minLength": 20,
                                           "description": "A Direwolf-generated Collision-Resistant Unique Identifier."
                                         },
                                         "elementId": {
                                           "type": "integer",
                                           "minimum": -1,
                                           "description": "The ElementId.Value property of this Element, defaulting to -1 (or ElementId.InvalidElementId) null."
                                         },
                                         "elementTypeId": {
                                           "type": "integer",
                                           "minimum": -1,
                                           "description": "The ElementTypeID.Id.Value of this Element, if applicable, defaulting to -1 (or ElementId.InvalidElementId) if null."
                                         },
                                         "elementName": {
                                           "type": "string",
                                           "description": "A string representing the name of the element."
                                         },
                                         "categoryType": {
                                           "type": "string",
                                           "description": "The string representation of the Category.CategoryType enum property, if applicable."
                                         },
                                         "builtInCategory": {
                                           "type": "string",
                                           "description": "The string representation of the Category.BuiltInCategory enum property, defaulting to BuiltInCategory.INVALID if null."
                                         },
                                         "parameters": {
                                           "type": "array",
                                           "items": {
                                             "type": "object",
                                             "properties": {
                                               "key": {
                                                 "type": "string",
                                                 "description": "The Definition.Name property on a given Parameter, defaulting to "undefined" if null."
                                               },
                                               "value": {
                                                 "type": "string",
                                                 "description": "A string value representing the Value held inside a given Parameter, defaulting to "undefined" if null."
                                               },
                                               "storageType": {
                                                 "type": "string",
                                                 "description": "A string representation of the StorageType enum property, default to StorageType.None if null."
                                               }
                                             },
                                             "required": ["key", "value", "storageType"]
                                           },
                                           "description": "An array of key-value pairs representing the Parameters held inside a given Element."
                                         }
                                       },
                                       "required": [
                                         "elementUniqueId",
                                         "id",
                                         "elementId",
                                         "elementTypeId",
                                         "elementName",
                                         "categoryType",
                                         "builtInCategory",
                                         "parameters"
                                       ]
                                     }
                                     """;
#pragma warning restore CA2211
}