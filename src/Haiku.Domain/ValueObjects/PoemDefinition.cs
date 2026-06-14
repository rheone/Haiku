using Haiku.Domain.Enums;

namespace Haiku.Domain.ValueObjects;

/// <summary>
/// The result of classifying a poem, carrying both identity and display metadata.
/// Populated by <c>ClassifierBuilder</c> from the matching classifier's properties.
/// </summary>
/// <remarks>
/// <para>Metadata fields (<see cref="TypeId"/>, <see cref="DisplayName"/>, etc.) come
/// directly from the classifier that matched — no external registry needed.</para>
/// </remarks>
public record PoemDefinition
{
    /// <summary>Gets the unique stable identifier from the matching classifier, e.g. "haiku", "syllable-pi".</summary>
    /// <value>A kebab-case string derived from the matching classifier.</value>
    public string TypeId { get; init; } = string.Empty;

    /// <summary>Gets the human-readable type name, e.g. "Syllable Pi".</summary>
    /// <value>The display name of the matched poem type.</value>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>Gets the description of the type's structural rules.</summary>
    /// <value>A prose description of the structural constraints.</value>
    public string Description { get; init; } = string.Empty;

    /// <summary>Gets the cultural lineage category.</summary>
    /// <value>A <see cref="PoemCategory"/> value indicating traditional or non-traditional origin.</value>
    public PoemCategory Category { get; init; }

    /// <summary>Gets the measurement axis for structural constraints.</summary>
    /// <value>A <see cref="PoemScaffold"/> value indicating syllable-based or word-based measurement.</value>
    public PoemScaffold Scaffold { get; init; }

    /// <summary>
    /// Gets the fixed syllable pattern per line, or <c>null</c> for sequence-based types.
    /// </summary>
    /// <value>An array of syllable counts per line, or <c>null</c> for sequence-based or word-based types.</value>
    public int[]? SyllablePattern { get; init; }

    /// <summary>
    /// Gets the fixed word-count pattern per line, or <c>null</c> for sequence-based types.
    /// Obsolete — all word-based types use dynamic sequence generation via <c>PatternGenerator</c>.
    /// </summary>
    /// <value>An array of word counts per line, or <c>null</c> for sequence-based or syllable-based types.</value>
    [Obsolete("Unused. All word-based types use dynamic sequence generation via PatternGenerator. Consider removal.")]
    public int[]? WordPattern { get; init; }

    /// <summary>
    /// Gets the backward-compatible enum value for DB storage.
    /// For traditional types this matches the enum; for new non-traditional types
    /// this returns <c>PoemType.Freeform</c> until the DB column is migrated to string.
    /// </summary>
    /// <value>A <see cref="PoemType"/> enum value.</value>
    public PoemType Type { get; init; }

    /// <summary>
    /// Gets the backward-compatible display name (same as <see cref="DisplayName"/>).
    /// </summary>
    /// <value>The display name string, identical to <see cref="DisplayName"/>.</value>
    public string Name { get; init; } = string.Empty;
}
