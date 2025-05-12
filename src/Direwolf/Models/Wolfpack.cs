using System.ComponentModel.DataAnnotations;
using Direwolf.Definitions;

namespace Direwolf.Models;

public record Wolfpack(
    [property: Key] Guid Id,
    [property: Timestamp] DateTime CreatedAt,
    Guid DocumentGuid
    
)
{
    public virtual PerformanceIndicators PerformanceIndicators { get; set; }
};