namespace Direwolf.Parsers.Tokens;

/// <summary>
/// Mappings to <see cref="FilteredElementCollector"/>
/// </summary>
public enum FilteringCondition
{
    Equals,
    NotEquals,
    GreaterThan,
    GreaterThanOrEquals,
    LessThan,
    LessThanOrEquals,
    Count,
    Passes,
    Matches,
    NotPasses,
    IsElementType,
    IsNotElementType,
    IsCurveDriven,
    IsViewIndependent,
    IsNotViewIndependent,
    IsElementId,
    IsNotElementId,
    IsView,
    IsSchedule,
    IsModelElement,
    IsAnnotativeElement,
    IsInternalElement,
    IsInvalidElement,
    InDesignOption,
    OfCategory,
    OfType,
    OnEnumeratorIndex,
    FirstElementId,
    FirstElement
    
}