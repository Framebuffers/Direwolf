namespace Direwolf.Definitions.Enums;

public enum DataType
{
    Null,
    Empty,
    Invalid,
    String,
    Numbers,
    Double,
    FloatingPoint,
    Boolean,
    Array,
    Object,
    DirewolfWolfpack, // non-standard types below
    DirewolfHowl,
    DirewolfException,
    VendorSpecific, // left for apps like Revit's internal objs
    EntityParameterDefinition, // a category value, a string Ex. [the string itself]
    EntityTaxonomyTag, // a category name. Ex. IfcUniqueId
    EntitySchemaDefinition // a category system or standard to define.
}