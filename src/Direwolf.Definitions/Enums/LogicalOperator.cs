namespace Direwolf.Definitions.Enums;

/// <summary>
///     Logic operations applied over a given eventCondition. Some require more
///     than one operand.
/// </summary>
public enum LogicalOperator
{
    // given two elements: p and q
    True,
    False,
    And, // true if both are true 
    Or, // true if either one of them is true
    Xor, // true if exactly one of them is true
    Xnor, // if and only if
    Not, // negative
    Nand, // it's not p or not q
    Nor, // it's not p and not q
    Null, // set without any values inside
    Exist, // exists in a given set
    NotExist, // does not exist in a given set
    ExistsInAll, // exists in all sets of a group
    IsUnique, // only one attribute exists in a set
    Empty, // set that contains a null
    NotEmpty, // value has something
    NullOrEmpty // either unallocated or allocated without anything inside
}