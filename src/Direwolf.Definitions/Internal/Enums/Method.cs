namespace Direwolf.Definitions.Internal.Enums;

/// <summary>
///     The bounds of the set that will be queried and returned as a result.
/// </summary>
public enum Method
{
    // standard
    Object,
    Array,
    String,
    Number,
    Boolean,
    // RVT
    ElementID,
    ParameterNone,
    ParameterInteger,
    ParameterDouble,
    ParameterString,
    ParameterElement,
    Category,
    Schedule,
    View,
    RevitDocument
}