using Haiku.Domain.Enums;

namespace Haiku.Domain.ValueObjects;

public record PoemDefinition
{
    public PoemType Type { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int[] SyllablePattern { get; init; } = Array.Empty<int>();
    public string? RhymeScheme { get; init; }
    public bool AllowVariableSyllables { get; init; }
}
