using Haiku.Domain.Enums;

namespace Haiku.Domain.ValueObjects;

/// <summary>
/// Defines the structural characteristics of a poem type, including its syllable pattern
/// and line constraints.
/// </summary>
public record PoemDefinition
{
    /// <summary>
    /// Gets the poetic form identifier.
    /// </summary>
    /// <value>A value from <see cref="PoemType"/> indicating the form.</value>
    public PoemType Type { get; init; }

    /// <summary>
    /// Gets the display name of the poem type.
    /// </summary>
    /// <value>A human-readable name for this form.</value>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets a description of the poem type's structural rules.
    /// </summary>
    /// <value>Explains the syllable pattern, rhyme scheme, or other constraints.</value>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets the required syllable counts per line, ordered by line position.
    /// </summary>
    /// <value>An array where each element represents the syllable count for the corresponding line.</value>
    public int[] SyllablePattern { get; init; } = Array.Empty<int>();
}
