namespace Direwolf.Definitions.Serialization.Schemas;

public static class WolfpackJsonSchema
{
    public const string Wolfpack = """
                                   {
                                     "$schema": "https://json-schema.org/draft/2020-12/schema",
                                     "type": "object",
                                     "properties": {
                                       "Id": {
                                         "type": "string",
                                         "format": "string",
                                         "description": "Unique identifier using a Collision-Resistant Unique Identifier format."
                                       },
                                       "MessageType": {
                                         "type": "string",
                                         "enum": ["GET", "POST", "PUT", "DELETE"],
                                         "description": "The operation being performed with this Wolfpack"
                                       },
                                       "ArgumentName": {
                                         "type": "string",
                                         "nullable": true,
                                         "description": "Optional name."
                                       },
                                       "Description": {
                                         "type": "string",
                                         "nullable": true,
                                         "description": "Optional description."
                                       },
                                       "Params": {
                                         "type": "object",
                                         "additionalProperties": {
                                           "type": "string"
                                         },
                                         "description": "An space to load any kind of API-specific property, outside the Direwolf context."
                                       },
                                       "Parameters": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/definitions/Wolfpack"
                                         },
                                         "description": "A list of elements that define transactions, operations or states to and from Direwolf. It is the transactional unit of Direwolf."
                                       }
                                     },
                                     "required": ["Id", "MessageType", "Parameters"],
                                     "definitions": {
                                       "Wolfpack": {
                                         "type": "object",
                                         "properties": {
                                           "Message": {
                                             "type": "string",
                                             "description": "A transaction, operation, and/or payload to be used within the Direwolf context."
                                            }
                                         },
                                         "required": ["Message"]
                                       }
                                     }
                                   }
                                   """;
}