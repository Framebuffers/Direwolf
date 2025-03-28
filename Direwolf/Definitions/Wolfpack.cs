using Direwolf.Contracts;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Direwolf.Definitions
{
    public readonly record struct Wolfpack() : IWolfpack
    {
        public Guid WolfpackUniqueId { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public string? Name { get; init; }
        public double? TimeTaken { get; init; }
        public string? Source { get; init; }
        public bool? WasCompleted { get;  init; }
        public int? ResultCount { get; init; }
        public string? Result { get; init; }
    }
}
