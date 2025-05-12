using System.ComponentModel.DataAnnotations;

namespace Direwolf.Models;

public record DirewolfElement(
    [property: Key] double Id,
    double IdValue,
    Guid IdUnique,
    string BuiltInCategory,
    string Name,
    [property: Timestamp] DateTime CapturedAt
)
{
    public virtual List<object>? Parameters { get; set; }
}